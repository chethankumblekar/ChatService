using ChatService.Application.DTOs;
using ChatService.Application.Interfaces;
using ChatService.Application.Mappings;
using ChatService.Domain.Entities;
using ChatService.Domain.Exceptions;
using ChatService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatService.Infrastructure.Services;

public sealed class ChatServiceImpl : IChatService
{
    private readonly IMessageRepository _messages;
    private readonly IUserRepository    _users;
    private readonly ILogger<ChatServiceImpl> _log;

    public ChatServiceImpl(IMessageRepository messages, IUserRepository users, ILogger<ChatServiceImpl> log)
        => (_messages, _users, _log) = (messages, users, log);

    public async Task<MessageDto> SendDirectMessageAsync(string senderId, string recipientId, string content, CancellationToken ct = default)
    {
        _ = await _users.GetByIdAsync(senderId, ct)    ?? throw new NotFoundException(nameof(User), senderId);
        _ = await _users.GetByIdAsync(recipientId, ct) ?? throw new NotFoundException(nameof(User), recipientId);
        var msg = Message.CreateDirect(senderId, recipientId, content);
        var saved = await _messages.AddAsync(msg, ct);
        _log.LogInformation("DM sent: {Id} from {From} to {To}", saved.Id, senderId, recipientId);
        return saved.ToDto();
    }

    public async Task<MessageDto> SendGroupMessageAsync(string senderId, Guid groupId, string content, CancellationToken ct = default)
    {
        _ = await _users.GetByIdAsync(senderId, ct) ?? throw new NotFoundException(nameof(User), senderId);
        var msg = Message.CreateGroup(senderId, groupId, content);
        var saved = await _messages.AddAsync(msg, ct);
        return saved.ToDto();
    }

    public async Task MarkMessageReadAsync(Guid messageId, string readerId, CancellationToken ct = default)
    {
        var msg = await _messages.GetByIdAsync(messageId, ct) ?? throw new NotFoundException(nameof(Message), messageId);
        if (msg.RecipientId != readerId) return;
        await _messages.MarkAsReadAsync(messageId, ct);
    }
}
