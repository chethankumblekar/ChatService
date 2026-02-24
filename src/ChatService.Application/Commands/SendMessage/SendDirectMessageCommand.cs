namespace ChatService.Application.Commands.SendMessage;
public record SendDirectMessageCommand(string SenderId, string RecipientId, string Content);
