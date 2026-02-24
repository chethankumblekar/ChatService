using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ChatService.Application.Interfaces;
using ChatService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Api.Hubs;

[Authorize]
public sealed class ChatHub : Hub
{
    // userId → set of active connectionIds (handles multiple tabs / devices)
    private static readonly ConcurrentDictionary<string, HashSet<string>> _presence = new();
    private static readonly object _lock = new();

    private readonly IChatService    _chat;
    private readonly IMessageRepository _messages;
    private readonly IUserRepository _users;
    private readonly ILogger<ChatHub> _log;

    public ChatHub(
        IChatService chat,
        IMessageRepository messages,
        IUserRepository users,
        ILogger<ChatHub> log)
    {
        _chat     = chat;
        _messages = messages;
        _users    = users;
        _log      = log;
    }

    private string UserId =>
        Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
        ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new HubException("Unauthenticated.");

    // ── Lifecycle ─────────────────────────────────────────────────────────────
    public override async Task OnConnectedAsync()
    {
        var userId = UserId;
        bool isNewUser;
        lock (_lock)
        {
            isNewUser = !_presence.ContainsKey(userId);
            _presence.AddOrUpdate(userId,
                _ => [Context.ConnectionId],
                (_, existing) => { existing.Add(Context.ConnectionId); return existing; });
        }
        if (isNewUser)
            await Clients.Others.SendAsync("UserOnline", userId);

        await Clients.Caller.SendAsync("OnlineUsers", _presence.Keys.ToList());
        await _users.UpdateLastSeenAsync(userId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = UserId;
        bool wentOffline;
        lock (_lock)
        {
            if (_presence.TryGetValue(userId, out var conns))
            {
                conns.Remove(Context.ConnectionId);
                wentOffline = conns.Count == 0;
                if (wentOffline) _presence.TryRemove(userId, out _);
            }
            else wentOffline = false;
        }
        if (wentOffline)
            await Clients.Others.SendAsync("UserOffline", userId);

        await _users.UpdateLastSeenAsync(userId);
        await base.OnDisconnectedAsync(exception);
    }

    // ── Client → Server methods ───────────────────────────────────────────────

    /// <summary>Send a direct message. Returns {id, senderId, recipientId, content, sentAt} to both parties.</summary>
    public async Task SendMessageToUser(string recipientId, string content)
    {
        var senderId = UserId;
        var dto = await _chat.SendDirectMessageAsync(senderId, recipientId, content);
        var payload = new { dto.Id, dto.SenderId, dto.RecipientId, dto.Content, dto.SentAt };

        await Clients.User(recipientId).SendAsync("ReceiveMessage", payload);
        await Clients.User(senderId).SendAsync("MessageSent", payload);   // echo to sender's other tabs
    }

    /// <summary>Send a message to a group.</summary>
    public async Task SendMessageToGroup(string groupId, string content)
    {
        if (!Guid.TryParse(groupId, out var gid))
            throw new HubException("Invalid group ID.");

        var dto     = await _chat.SendGroupMessageAsync(UserId, gid, content);
        var payload = new { dto.Id, dto.SenderId, dto.GroupId, dto.Content, dto.SentAt };
        await Clients.Group(groupId).SendAsync("ReceiveGroupMessage", payload);
    }

    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        await Clients.Group(groupId).SendAsync("UserJoinedGroup", UserId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
        await Clients.Group(groupId).SendAsync("UserLeftGroup", UserId, groupId);
    }

    /// <summary>Mark a message as read. Sends read receipt to the sender.</summary>
    public async Task MarkMessageRead(Guid messageId)
    {
        var readerId = UserId;
        var msg = await _messages.GetByIdAsync(messageId);
        if (msg is null) return;

        await _chat.MarkMessageReadAsync(messageId, readerId);
        await Clients.User(msg.SenderId).SendAsync("MessageRead", messageId, readerId);
    }

    /// <summary>Broadcast typing indicator (throttle on client side to 2s).</summary>
    public Task Typing(string recipientId) =>
        Clients.User(recipientId).SendAsync("UserTyping", UserId);

    /// <summary>Check if a user is currently online.</summary>
    public Task<bool> IsUserOnline(string userId) =>
        Task.FromResult(_presence.ContainsKey(userId));
}
