namespace ChatService.Application.DTOs;
public record MessageDto(Guid Id, string SenderId, string? RecipientId, Guid? GroupId, string Content, DateTime SentAt, DateTime? ReadAt);
public record ConversationDto(string OtherUserId, string OtherUserFirstName, string OtherUserLastName, string? OtherUserAvatarUrl, string LastMessageContent, DateTime LastMessageSentAt, string LastMessageSenderId, int UnreadCount);
