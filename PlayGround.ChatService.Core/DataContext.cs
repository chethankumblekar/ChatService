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

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=VT10042,1433;Initial Catalog=ChatService;Persist Security Info=False;User ID=proadmin;Password=admin@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.Email)
                    .IsRequired();

                entity.HasMany(u => u.Messages)
                    .WithOne(m => m.Sender)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable("users");
            });

            modelBuilder.Entity<Group>(entity =>
            {
                entity.HasKey(g => g.Id);

                entity.Property(g => g.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(g => g.Members)
                    .WithMany(u => u.Groups) 
                    .UsingEntity(join => join.ToTable("user_groups")); 

                entity.HasMany(g => g.Messages)
                    .WithOne(m => m.Group)
                    .HasForeignKey(m => m.GroupId)
                    .OnDelete(DeleteBehavior.Cascade); 
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Content)
                    .IsRequired();

                entity.HasOne(m => m.Recipient)
                    .WithMany()
                    .HasForeignKey(m => m.RecipientId)
                    .OnDelete(DeleteBehavior.Restrict); 

                entity.HasOne(m => m.Group)
                    .WithMany(g => g.Messages)
                    .HasForeignKey(m => m.GroupId)
                    .IsRequired(false);

                entity.ToTable("messages");
            });
        }
    }
}
