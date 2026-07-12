using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using ProjectCore.Application.UseCases.Permissions.Scan;

namespace ProjectCore.Presentation.API.Permissions;

public sealed class ApiPermissionScanner : IPermissionScanner
{
    private readonly ILogger<ApiPermissionScanner> _logger;

    public ApiPermissionScanner(ILogger<ApiPermissionScanner> logger)
    {
        _logger = logger;
    }

    public IEnumerable<PermissionScanResult> Scan()
    {
        var assembly = Assembly.GetExecutingAssembly();
        _logger.LogInformation("Scanning assembly: {AssemblyName}", assembly.GetName().Name);

        var controllerTypes = assembly
            .GetTypes()
            .Where(t =>
                typeof(ControllerBase).IsAssignableFrom(t) &&
                !t.IsAbstract)
            .ToList();

        _logger.LogInformation("Found {Count} controller(s): {Controllers}",
            controllerTypes.Count,
            string.Join(", ", controllerTypes.Select(t => t.Name)));

        foreach (var controller in controllerTypes)
        {
            var module = controller.Name.Replace("Controller", "");

            var actions = controller
                .GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Where(m => !m.IsDefined(typeof(NonActionAttribute)))
                .ToList();

            _logger.LogInformation("  {Module}Controller -> {Count} action(s): {Actions}",
                module, actions.Count,
                string.Join(", ", actions.Select(a => a.Name)));

            foreach (var action in actions)
            {
                yield return new PermissionScanResult
                {
                    Module = module,
                    Action = action.Name
                };
            }
        }
    }
}
