using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectCore.Application.UseCases.Roles.Queries.GetAllRoles;
using ProjectCore.Application.UseCases.Users.Commands.CreateUser;
using ProjectCore.Application.UseCases.Users.Commands.DeleteUser;
using ProjectCore.Application.UseCases.Users.Commands.UpdateUser;
using ProjectCore.Application.UseCases.Users.Queries.GetDataUsers;
using ProjectCore.Application.UseCases.Users.Queries.GetUserById;
using ProjectCore.Presentation.API.Models.Requests;
using ProjectCore.Presentation.API.Models.Responses;

namespace ProjectCore.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get paginated users with optional search/filter/sort.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetUsersRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetUsersQuery
        {
            Keyword        = request.Keyword,
            UserName       = request.UserName,
            Email          = request.Email,
            FullName       = request.FullName,
            Gender         = request.Gender,
            RoleId         = request.RoleId,
            SortBy         = request.SortBy,
            SortDescending = request.SortDescending,
            Page           = request.Page,
            PageSize       = request.PageSize,
        }, cancellationToken);

        return Ok(ApiResponse<object>.Ok(result));
    }

    /// <summary>Get user by ID.</summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        // Handler throws UserNotFoundException → ExceptionMiddleware returns 404
        var user = await _mediator.Send(new GetUserByIdQuery { UserId = id }, cancellationToken);
        return Ok(ApiResponse<object>.Ok(user));
    }

    /// <summary>Get all roles (for assignment dropdowns).</summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var roles = await _mediator.Send(new GetAllRolesQuery(), cancellationToken);
        return Ok(ApiResponse<object>.Ok(roles));
    }

    /// <summary>Create a new user account.</summary>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var userId = await _mediator.Send(new CreateUserCommand
        {
            UserName  = request.UserName,
            Email     = request.Email,
            Password  = request.Password,   // plain-text — hashed inside the handler
            CreatedBy = CurrentUserId,
        }, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = userId },
            ApiResponse<object>.Ok(new { id = userId }, "User created successfully"));
    }

    /// <summary>Update an existing user's profile.</summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new UpdateUserCommand
        {
            Id          = id,
            FullName    = request.FullName,
            PhoneNumber = request.PhoneNumber,
            Gender      = request.Gender,
            DateOfBirth = request.DateOfBirth,
            Address     = request.Address,
            AvatarUrl   = request.AvatarUrl,
            UpdatedBy   = CurrentUserId,
        }, cancellationToken);

        return Ok(ApiResponse.Ok("User updated successfully"));
    }

    /// <summary>Delete a user by ID.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteUserCommand { Id = id }, cancellationToken);
        return Ok(ApiResponse.Ok("User deleted successfully"));
    }
}
