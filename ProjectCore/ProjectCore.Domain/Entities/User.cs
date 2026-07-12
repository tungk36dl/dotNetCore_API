using ProjectCore.Domain.Entities;
using ProjectCore.Domain.Exceptions;
using ProjectCore.Domain.ValueObjects.User;

namespace ProjectCore.Domain.Entities;

public class User : DomainEntity<Guid>
{
    public UserName UserName { get; private set; }
    public Email Email { get; private set; }
    public string PasswordHash { get; private set; }

    public FullName? FullName { get; private set; }
    public PhoneNumber? PhoneNumber { get; private set; }
    public Gender? Gender { get; private set; } 
    public DateOnly? DateOfBirth { get; private set; }
    public string? Address { get; private set; }
    public Avatar? Avatar { get; private set; }

    private readonly List<UserRole> _userRoles = new();
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    protected User() { } 

    public User(
        Guid id,
        UserName userName,
        Email email,
        string passwordHash,
        Guid createdBy)
        : base(id, createdBy)
    {
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
    }

    // ===== Behavior =====
    /// <summary>
    /// Chỉnh sửa thông tin cá nhân người dùng
    /// </summary>
    /// <param name="fullName"></param>
    /// <param name="phoneNumber"></param>
    /// <param name="gender"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="address"></param>
    /// <param name="avatar"></param>
    public void UpdateProfile(
    FullName? fullName,
    PhoneNumber? phoneNumber,
    Gender? gender,
    DateOnly? dateOfBirth,
    string? address,
    Avatar? avatar,
    Guid updatedBy)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Gender = gender;
        DateOfBirth = dateOfBirth;
        Address = address;
        Avatar = avatar;

        MarkUpdated(updatedBy);
    }

    /// <summary>
    /// Thêm vai trò cho người dùng
    /// </summary>
    /// <param name="roleId"></param>
    /// <param name="assignedBy"></param>
    public void AssignRole(Guid roleId, Guid assignedBy)
    {
        if (_userRoles.Any(x => x.RoleId == roleId))
            return;

        _userRoles.Add(new UserRole(Id, roleId, assignedBy));
    }

    /// <summary>
    /// Xóa vai trò khỏi người dùng
    /// </summary>
    /// <param name="roleId"></param>
    public void RemoveRole(Guid roleId)
    {
        var role = _userRoles.FirstOrDefault(x => x.RoleId == roleId);
        if (role == null)
        {
            throw new UserDoesNotHaveRoleException();
        }
        if (_userRoles.Count <= 1)
        {
            throw new UserMustHaveAtLeastOneRoleException();
        }
         _userRoles.Remove(role);
    }
}
