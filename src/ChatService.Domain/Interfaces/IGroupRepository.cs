using ChatService.Domain.Entities;
namespace ChatService.Domain.Interfaces;
public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetByUserAsync(string userId, CancellationToken ct = default);
    Task<Group> AddAsync(Group group, CancellationToken ct = default);
    Task UpdateAsync(Group group, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
