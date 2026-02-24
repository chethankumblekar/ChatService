using ChatService.Domain.Entities;
namespace ChatService.Domain.Interfaces;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(string userId, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default);
    Task<IEnumerable<User>> SearchAsync(string query, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task UpdateAsync(User user, CancellationToken ct = default);
    Task UpdateLastSeenAsync(string userId, CancellationToken ct = default);
}
