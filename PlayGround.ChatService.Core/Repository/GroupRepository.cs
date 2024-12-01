using Microsoft.EntityFrameworkCore;
using PlayGround.ChatService.Core;
using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Infrastructure.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly DataContext _context;

        public GroupRepository(DataContext context)
        {
            _context = context;
        }

        public async Task AddGroupAsync(Group group)
        {
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(string groupId)
        {
            var group = await _context.Groups.FindAsync(groupId);

            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }

            throw new KeyNotFoundException();
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups
                            .Include(g => g.Members)
                            .ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(Guid id)
        {
            return await _context.Groups
                            .Include(g => g.Members)
                            .Include(g => g.Messages)
                            .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }
    }
}
