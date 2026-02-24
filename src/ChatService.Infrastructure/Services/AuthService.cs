using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChatService.Application.Interfaces;
using ChatService.Domain.Entities;
using ChatService.Domain.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ChatService.Infrastructure.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration  _config;
    private readonly ILogger<AuthService> _log;

    public AuthService(IUserRepository users, IConfiguration config, ILogger<AuthService> log)
        => (_users, _config, _log) = (users, config, log);

    public async Task<string> AuthenticateWithGoogleAsync(string googleIdToken, CancellationToken ct = default)
    {
        // BUG FIX: validate Audience to prevent tokens from other Google apps being accepted
        var payload = await GoogleJsonWebSignature.ValidateAsync(googleIdToken,
            new GoogleJsonWebSignature.ValidationSettings { Audience = [_config["Authentication:GoogleClientId"]] });

        var user = await _users.GetByEmailAsync(payload.Email, ct);
        if (user is null)
        {
            user = User.Create(payload.Email, payload.GivenName ?? "User", payload.FamilyName ?? "", payload.Picture);
            await _users.AddAsync(user, ct);
            _log.LogInformation("New user registered: {Email}", payload.Email);
        }
        else
        {
            user.UpdateProfile(payload.GivenName ?? user.FirstName, payload.FamilyName ?? user.LastName, payload.Picture);
            await _users.UpdateAsync(user, ct);
        }

        return IssueJwt(user, payload.Picture ?? "");
    }

    private string IssueJwt(User user, string picture)
    {
        var key    = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Authentication:JwtSecret"]!));
        var creds  = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiry = _config.GetValue<int>("Authentication:TokenExpiryMinutes", 60);

        // BUG FIX: "sub" claim = email, used as userId in hub IUserIdProvider
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub,        user.Email),
            new Claim(JwtRegisteredClaimNames.Email,      user.Email),
            new Claim(JwtRegisteredClaimNames.GivenName,  user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new Claim(JwtRegisteredClaimNames.Picture,    picture),
            new Claim(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Authentication:Issuer"],
            audience: _config["Authentication:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiry),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
