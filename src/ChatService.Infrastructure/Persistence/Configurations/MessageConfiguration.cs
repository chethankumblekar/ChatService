using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Persistence.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.ToTable("messages");
        b.HasKey(m => m.Id);
        b.Property(m => m.Id).ValueGeneratedNever();
        b.Property(m => m.Content).IsRequired().HasMaxLength(4000);
        b.HasIndex(m => m.SenderId);
        b.HasIndex(m => m.RecipientId);
        b.HasIndex(m => m.SentAt);
        b.HasQueryFilter(m => !m.IsDeleted);  // global soft-delete
    }
}
