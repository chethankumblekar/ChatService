using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(u => u.Email);
        b.Property(u => u.Email).HasMaxLength(256);
        b.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
        b.Property(u => u.LastName).IsRequired().HasMaxLength(50);
        b.Property(u => u.AvatarUrl).HasMaxLength(2048);
        b.HasIndex(u => u.Email).IsUnique();

        b.HasMany(u => u.SentMessages)
            .WithOne(m => m.Sender)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(u => u.ReceivedMessages)
            .WithOne(m => m.Recipient)
            .HasForeignKey(m => m.RecipientId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
