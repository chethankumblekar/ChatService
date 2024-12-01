using Microsoft.EntityFrameworkCore;
using PlayGround.ChatService.Model;

namespace PlayGround.ChatService.Infrastructure
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Group> Groups { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=VT10042,1433;Initial Catalog=ChatService;Persist Security Info=False;User ID=admin;Password=admin@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>()
               .HasOne<User>()
               .WithMany(u => u.Messages)
               .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<Group>()
                .HasMany(g => g.Members)
                .WithMany(u => u.Groups);

            modelBuilder.Entity<Message>()
                .HasOne<Group>()
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.GroupId);
        }
    }
}
