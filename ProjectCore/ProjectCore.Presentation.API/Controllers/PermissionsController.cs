using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectCore.Application.UseCases.Permissions.Scan;
using ProjectCore.Presentation.API.Models.Responses;

namespace ProjectCore.Presentation.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PermissionsController(IMediator mediator) => _mediator = mediator;

    /// <summary>Scan all API controllers and synchronise discovered permissions to the database.</summary>
    [HttpPost("scan")]
    public async Task<IActionResult> Scan(CancellationToken cancellationToken)
    {
        var userId = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : Guid.Empty;

        await _mediator.Send(new SyncPermissionsCommand { IssuedByUserId = userId }, cancellationToken);

        return Ok(ApiResponse.Ok("Permissions scanned and synchronised successfully"));
    }
}
