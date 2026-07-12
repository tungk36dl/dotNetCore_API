using Microsoft.Extensions.Logging;
using ProjectCore.Application.Common.Configuration;
using ProjectCore.Application.Logging;
using ProjectCore.Application.Common.Security;
using ProjectCore.Application.Interfaces;
using ProjectCore.Application.UseCases.Permissions.Scan;
using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Interfaces.PermissionRepository;
using ProjectCore.Domain.Interfaces.RoleRepository;
using ProjectCore.Domain.Interfaces.UserRepository;
using ProjectCore.Domain.ValueObjects.Role;
using ProjectCore.Domain.ValueObjects.User;


namespace ProjectCore.Application.UseCases.SeedData;
public sealed class SeedDataHandler
{
    private readonly ILogger<SeedDataHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IPermissionScanner _permissionScanner;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdminSeedConfig _adminSeedConfig;
    private readonly IAdminRoleSeedConfig _adminRoleSeedConfig;

    public SeedDataHandler(
        ILogger<SeedDataHandler> logger,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        IPermissionScanner permissionScanner,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        IAdminSeedConfig adminSeedConfig,
        IAdminRoleSeedConfig adminRoleSeedConfig)
    {
        _logger = logger;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _permissionScanner = permissionScanner;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _adminSeedConfig = adminSeedConfig;
        _adminRoleSeedConfig = adminRoleSeedConfig;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(LogEvents.BootstrapStarted, "=== System Bootstrap Started ===");
        _logger.LogInformation("Admin config: UserName={UserName}, Email={Email}, FullName={FullName}",
            _adminSeedConfig.UserName, _adminSeedConfig.Email, _adminSeedConfig.FullName);
        _logger.LogInformation("Admin role config: Name={RoleName}, Description={Description}",
            _adminRoleSeedConfig.Name, _adminRoleSeedConfig.Description);

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        _logger.LogInformation("Transaction started.");

        try
        {
            var systemUserId = SystemAccount.Id;
            _logger.LogInformation("SystemAccount.Id = {SystemUserId}", systemUserId);

            // 1. Sync permissions
            await SyncPermissionsAsync(systemUserId, cancellationToken);
            // Flush permissions to DB (within transaction) so they can be queried in step 3
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Flushed permissions to DB.");

            // 2. Ensure admin role
            var adminRole = await EnsureAdminRoleAsync(systemUserId, cancellationToken);
            // Flush role to DB so Update() in step 3 won't change state from Added to Modified
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Flushed admin role to DB.");

            // 3. Ensure admin has all permissions
            await EnsureAdminHasAllPermissionsAsync(adminRole, systemUserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Flushed role-permissions to DB.");

            // 4. Ensure admin user
            await EnsureAdminUserAsync(adminRole.Id, systemUserId, cancellationToken);

            _logger.LogInformation("Committing transaction...");
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation(LogEvents.BootstrapCompleted, "=== System Bootstrap Completed Successfully ===");
        }
        catch (Exception ex)
        {
            _logger.LogError(LogEvents.BootstrapFailed, ex, "System bootstrap FAILED - rolling back transaction");
            await _unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }


    // ========================== PERMISSIONS ==========================

    private async Task SyncPermissionsAsync(Guid systemUserId, CancellationToken ct)
    {
        _logger.LogInformation("[Step 1/4] Scanning permissions from API controllers...");

        var scanned = _permissionScanner.Scan().ToList();
        _logger.LogInformation("Scanner found {Count} permission(s) from controllers.", scanned.Count);

        foreach (var p in scanned)
        {
            _logger.LogDebug("  Scanned: {Module}.{Action}", p.Module, p.Action);
        }

        var newCount = 0;
        foreach (var p in scanned)
        {
            if (await _permissionRepository.ExistsAsync(p.Module, p.Action, ct))
            {
                _logger.LogDebug("  Permission {Module}.{Action} already exists, skipping.", p.Module, p.Action);
                continue;
            }

            var permission = new Permission(Guid.NewGuid(), p.Module, p.Action, systemUserId);
            await _permissionRepository.AddAsync(permission, ct);
            newCount++;
            _logger.LogInformation("  + Created permission: {Module}.{Action} (Id={Id})", p.Module, p.Action, permission.Id);
        }

        _logger.LogInformation("[Step 1/4] Done. {NewCount} new permission(s) created, {ExistingCount} already existed.",
            newCount, scanned.Count - newCount);
    }

    // ========================== ROLE ==========================

    private async Task<Role> EnsureAdminRoleAsync(Guid systemUserId, CancellationToken ct)
    {
        _logger.LogInformation("[Step 2/4] Ensuring admin role '{RoleName}'...", _adminRoleSeedConfig.Name);

        var roleName = new RoleName(_adminRoleSeedConfig.Name);

        var role = await _roleRepository.GetByNameAsync(roleName, ct);
        if (role != null)
        {
            _logger.LogInformation("[Step 2/4] Admin role already exists (Id={RoleId}). Skipping creation.", role.Id);
            return role;
        }

        role = new Role(
            Guid.NewGuid(),
            roleName,
            _adminRoleSeedConfig.Description ?? "System administrator",
            systemUserId);

        await _roleRepository.AddAsync(role, ct);
        _logger.LogInformation("[Step 2/4] Created admin role '{RoleName}' (Id={RoleId}).", roleName.Value, role.Id);
        return role;
    }

    // ========================== ROLE -> PERMISSION ==========================

    private async Task EnsureAdminHasAllPermissionsAsync(
        Role adminRole,
        Guid systemUserId,
        CancellationToken ct)
    {
        _logger.LogInformation("[Step 3/4] Assigning all permissions to admin role (Id={RoleId})...", adminRole.Id);

        var allPermissions = await _permissionRepository.GetAllAsync(ct);
        var permissionList = allPermissions.ToList();
        _logger.LogInformation("  Total permissions in DB: {Count}", permissionList.Count);
        _logger.LogInformation("  Admin role currently has {Count} permission(s).", adminRole.RolePermissions.Count);

        var addedCount = 0;
        foreach (var p in permissionList)
        {
            var hadBefore = adminRole.RolePermissions.Count;
            adminRole.AddPermission(p.Id, systemUserId);
            if (adminRole.RolePermissions.Count > hadBefore)
            {
                addedCount++;
                _logger.LogDebug("  + Assigned: {Module}.{Action}", p.Module, p.Action);
            }
        }

        // Do NOT call _roleRepository.Update() here.
        // adminRole is already tracked by EF (loaded via GetByNameAsync or AddAsync).
        // DbContext.Update() would cascade Modified state to new RolePermission entities,
        // causing them to be UPDATEd instead of INSERTed → DbUpdateConcurrencyException.
        // EF change tracker auto-detects the new RolePermissions on SaveChanges.
        _logger.LogInformation("[Step 3/4] Done. {AddedCount} new permission(s) assigned, {SkippedCount} already assigned.",
            addedCount, permissionList.Count - addedCount);
    }

    // ========================== USER ==========================

    private async Task EnsureAdminUserAsync(Guid adminRoleId, Guid systemUserId, CancellationToken ct)
    {
        _logger.LogInformation("[Step 4/4] Ensuring admin user '{UserName}'...", _adminSeedConfig.UserName);

        var userName = new UserName(_adminSeedConfig.UserName);

        var existing = await _userRepository.GetByUserNameAsync(userName, ct);
        if (existing != null)
        {
            _logger.LogInformation("  Admin user already exists (Id={UserId}). Ensuring role assignment...", existing.Id);
            _logger.LogInformation("  User currently has {Count} role(s).", existing.UserRoles.Count);

            var hadRole = existing.UserRoles.Any(ur => ur.RoleId == adminRoleId);
            existing.AssignRole(adminRoleId, systemUserId);
            // Do NOT call _userRepository.Update() - entity is already tracked by EF.
            // Same reason as step 3: Update() cascades Modified state to new UserRole.

            if (hadRole)
                _logger.LogInformation("  Admin role already assigned. No changes.");
            else
                _logger.LogInformation("  + Admin role assigned to existing user.");

            _logger.LogInformation("[Step 4/4] Done (existing user).");
            return;
        }

        _logger.LogInformation("  Admin user not found. Creating new admin user...");
        var hash = _passwordHasher.Hash(_adminSeedConfig.Password);

        var admin = new User(
            Guid.NewGuid(),
            userName,
            new Email(_adminSeedConfig.Email),
            hash,
            systemUserId);

        if (!string.IsNullOrWhiteSpace(_adminSeedConfig.FullName))
        {
            admin.UpdateProfile(
                new FullName(_adminSeedConfig.FullName),
                null, null, null, null, null,
                systemUserId);
            _logger.LogInformation("  Set FullName: {FullName}", _adminSeedConfig.FullName);
        }

        admin.AssignRole(adminRoleId, systemUserId);
        _logger.LogInformation("  + Admin role assigned to new user.");

        await _userRepository.AddAsync(admin, ct);
        _logger.LogInformation("[Step 4/4] Done. Created admin user '{UserName}' (Id={UserId}).", userName.Value, admin.Id);
    }
}
