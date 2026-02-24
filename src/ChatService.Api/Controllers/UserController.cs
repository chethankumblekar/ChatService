using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatService.Application.DTOs;
using ChatService.Application.Mappings;
using ChatService.Application.Queries.GetConversations;
using ChatService.Application.Queries.GetMessages;
using ChatService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public sealed class UserController : ControllerBase
{
    private readonly IUserRepository         _users;
    private readonly IMessageRepository      _messages;
    private readonly GetDirectMessagesHandler _getMessages;
    private readonly GetConversationsHandler  _getConversations;

    public UserController(
        IUserRepository users,
        IMessageRepository messages,
        GetDirectMessagesHandler getMessages,
        GetConversationsHandler getConversations)
    {
        _users            = users;
        _messages         = messages;
        _getMessages      = getMessages;
        _getConversations = getConversations;
    }

    private string CurrentUserId =>
        User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    /// GET api/user?search=
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsers([FromQuery] string? search, CancellationToken ct)
    {
        var users = string.IsNullOrWhiteSpace(search)
            ? await _users.GetAllAsync(ct)
            : await _users.SearchAsync(search, ct);

        return Ok(users
            .Where(u => u.Email != CurrentUserId)
            .Select(u => new UserDto(u.Email, u.FirstName, u.LastName, u.Email, u.AvatarUrl, u.LastSeenAt)));
    }

    /// GET api/user/conversations
    [HttpGet("conversations")]
    [ProducesResponseType(typeof(IEnumerable<ConversationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConversations(CancellationToken ct)
    {
        var result = await _getConversations.HandleAsync(new GetConversationsQuery(CurrentUserId), ct);
        return Ok(result);
    }

    /// GET api/user/messages/{recipientId}?skip=0&take=50
    [HttpGet("messages/{recipientId}")]
    [ProducesResponseType(typeof(IEnumerable<MessageDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMessages(string recipientId, [FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
    {
        var result = await _getMessages.HandleAsync(new GetDirectMessagesQuery(CurrentUserId, recipientId, skip, take), ct);
        return Ok(result);
    }

    /// POST api/user/messages/{senderId}/read
    [HttpPost("messages/{senderId}/read")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> MarkRead(string senderId, CancellationToken ct)
    {
        await _messages.MarkAllReadAsync(senderId, CurrentUserId, ct);
        return NoContent();
    }
}
