using ChatService.Application.DTOs;
using ChatService.Domain.Entities;
namespace ChatService.Application.Mappings;
public static class MessageMappings
{
    public static MessageDto ToDto(this Message m) =>
        new(m.Id, m.SenderId, m.RecipientId, m.GroupId, m.Content, m.SentAt, m.ReadAt);
}
