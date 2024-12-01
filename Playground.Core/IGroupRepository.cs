using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Core
{
    public interface IGroupRepository
    {
        Task AddGroupAsync(Group group);
        Task UpdateGroupAsync(Group group);
        Task<Group?> GetGroupByIdAsync(Guid groupId);
        Task DeleteGroupAsync(string groupId);
        Task<IEnumerable<Group>> GetAllGroupsAsync();

    }
}
