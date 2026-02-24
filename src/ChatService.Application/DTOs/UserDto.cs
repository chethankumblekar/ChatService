namespace ChatService.Application.DTOs;
public record UserDto(string Id, string FirstName, string LastName, string Email, string? AvatarUrl, DateTime LastSeenAt);
