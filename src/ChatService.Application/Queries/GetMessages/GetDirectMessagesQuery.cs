namespace ChatService.Application.Queries.GetMessages;
public record GetDirectMessagesQuery(string UserId, string RecipientId, int Skip = 0, int Take = 50);
