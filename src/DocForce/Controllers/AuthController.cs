using DocForce.API.Extensions;
using DocForce.Application.Common.Dispatcher.Interfaces;
using DocForce.Application.Features.Auth.Commands;
using Microsoft.AspNetCore.Mvc;

namespace DocForce.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;
    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    [Route("register/")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost]
    [Route("login/")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost]
    [Route("refresh/")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }

    [HttpPost]
    [Route("revoke/")]
    public async Task<IActionResult> Revoke([FromBody] RevokeRefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(command, cancellationToken);

        return result.ToActionResult();
    }
}
