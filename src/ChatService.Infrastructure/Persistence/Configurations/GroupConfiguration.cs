using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatService.Infrastructure.Persistence.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> b)
    {
        b.ToTable("groups");
        b.HasKey(g => g.Id);
        b.Property(g => g.Id).ValueGeneratedNever();
        b.Property(g => g.Name).IsRequired().HasMaxLength(100);
        b.Property(g => g.Description).HasMaxLength(500);
        b.HasMany(g => g.Members).WithMany()
            .UsingEntity(j => j.ToTable("group_members"));

        b.HasMany(g => g.Messages).WithOne(m => m.Group)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}
