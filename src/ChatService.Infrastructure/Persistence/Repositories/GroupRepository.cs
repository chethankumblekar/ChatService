using ChatService.Domain.Entities;
using ChatService.Domain.Exceptions;
using ChatService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Infrastructure.Persistence.Repositories;

public sealed class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _db;
    public GroupRepository(AppDbContext db) => _db = db;

    public Task<Group?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        _db.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IEnumerable<Group>> GetByUserAsync(string userId, CancellationToken ct = default) =>
        await _db.Groups.Include(g => g.Members).Where(g => g.Members.Any(u => u.Email == userId)).ToListAsync(ct);

    public async Task<Group> AddAsync(Group group, CancellationToken ct = default)
    {
        _db.Groups.Add(group); await _db.SaveChangesAsync(ct); return group;
    }

    public async Task UpdateAsync(Group group, CancellationToken ct = default)
    {
        _db.Groups.Update(group); await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var group = await _db.Groups.FindAsync(new object[] { id }, ct)
            ?? throw new NotFoundException(nameof(Group), id);   // BUG FIX: throw was OUTSIDE null check in original
        _db.Groups.Remove(group);
        await _db.SaveChangesAsync(ct);
    }
}
