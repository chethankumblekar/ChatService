namespace ChatService.Application.Interfaces;
public interface IAuthService
{
    Task<string> AuthenticateWithGoogleAsync(string googleIdToken, CancellationToken ct = default);
}
