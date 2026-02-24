using ChatService.Application.DTOs;
using ChatService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth) => _auth = auth;

    /// POST api/auth/google
    /// Body: { "token": "<google_id_token>" }
    /// Returns: { "token": "<app_jwt>" }
    [AllowAnonymous]
    [HttpPost("google")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Google([FromBody] GoogleAuthRequest request, CancellationToken ct)
    {
        var token = await _auth.AuthenticateWithGoogleAsync(request.Token, ct);
        return Ok(new AuthResponse(token));
    }

    /// GET api/auth/me  — returns JWT claims of the current user
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me() =>
        Ok(User.Claims.ToDictionary(c => c.Type, c => c.Value));
}
