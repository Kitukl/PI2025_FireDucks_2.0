using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class CommentsConfiguration : IEntityTypeConfiguration<Comments>
{
    public void Configure(EntityTypeBuilder<Comments> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Description).IsRequired();
        builder.Property(c => c.CreationDate).IsRequired();
        builder.Property(c => c.TaskId).IsRequired();
        builder.Property(c => c.UserId).IsRequired();

        builder.HasOne(c => c.Task)
            .WithMany(t => t.CommentsList)
            .HasForeignKey(c => c.TaskId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.User)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}