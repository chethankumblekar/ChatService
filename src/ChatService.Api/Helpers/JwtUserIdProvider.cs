using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Helpers;

/// <summary>
/// Maps SignalR connections to the "sub" JWT claim (user email).
/// Fixes original bug where ClaimTypes.Email (URI) was used instead of "sub".
/// </summary>
public sealed class JwtUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection) =>
        connection.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
}
