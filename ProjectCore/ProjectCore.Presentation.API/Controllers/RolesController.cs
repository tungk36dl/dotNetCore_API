using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectCore.Application.UseCases.Roles.Commands.CreateRole;
using ProjectCore.Application.UseCases.Roles.Commands.DeleteRole;
using ProjectCore.Application.UseCases.Roles.Commands.UpdateRole;
using ProjectCore.Application.UseCases.Roles.Queries.GetDataRoles;
using ProjectCore.Application.UseCases.Roles.Queries.GetRoleById;
using ProjectCore.Presentation.API.Models.Requests;
using ProjectCore.Presentation.API.Models.Responses;

namespace ProjectCore.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;

    public RolesController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get paginated roles with optional search/sort.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetRolesRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetRolesQuery
        {
            Keyword        = request.Keyword,
            Name           = request.Name,
            SortBy         = request.SortBy,
            SortDescending = request.SortDescending,
            Page           = request.Page,
            PageSize       = request.PageSize,
        }, cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Get role by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // Handler throws RoleNotFoundException → ExceptionMiddleware returns 404
        var role = await _mediator.Send(new GetRoleByIdQuery { Id = id }, cancellationToken);
        return Ok(ApiResponse<object>.Ok(role));
    }

    /// <summary>Create a new role.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        var roleId = await _mediator.Send(new CreateRoleCommand
        {
            RoleName      = request.RoleName,
            Description   = request.Description,
            CreatedBy     = CurrentUserId,
            PermissionIds = request.PermissionIds ?? [],
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = roleId },
            ApiResponse<object>.Ok(new { id = roleId }, "Role created successfully"));
    }

    /// <summary>Update an existing role.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateRoleCommand
        {
            Id            = id,
            Name          = request.Name,
            Description   = request.Description,
            UpdatedBy     = CurrentUserId,
            PermissionIds = request.PermissionIds,
        }, cancellationToken);

        return Ok(ApiResponse.Ok("Role updated successfully"));
    }

    /// <summary>Delete a role.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteRoleCommand { Id = id }, cancellationToken);
        return Ok(ApiResponse.Ok("Role deleted successfully"));
    }
}
