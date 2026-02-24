using ChatService.Domain.Entities;
using ChatService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ChatService.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    private readonly ILogger<UserRepository> _log;
    public UserRepository(AppDbContext db, ILogger<UserRepository> log) => (_db, _log) = (db, log);

    public Task<User?> GetByIdAsync(string userId, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == userId, ct);

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _db.Users.FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default) =>
        await _db.Users.OrderBy(u => u.FirstName).ToListAsync(ct);

    public async Task<IEnumerable<User>> SearchAsync(string query, CancellationToken ct = default)
    {
        var q = query.ToLower();
        return await _db.Users.Where(u =>
            u.FirstName.ToLower().Contains(q) ||
            u.LastName.ToLower().Contains(q)  ||
            u.Email.ToLower().Contains(q))
            .Take(20).ToListAsync(ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
        _log.LogInformation("User created: {Email}", user.Email);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateLastSeenAsync(string userId, CancellationToken ct = default) =>
        await _db.Users.Where(u => u.Email == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastSeenAt, DateTime.UtcNow), ct);
}
