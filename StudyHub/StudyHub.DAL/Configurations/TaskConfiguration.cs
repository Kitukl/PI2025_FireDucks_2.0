using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = StudyHub.DAL.Entities.Task;

namespace StudyHub.DAL.Configurations;

public class TaskConfiguration : IEntityTypeConfiguration<Task>
{
    public void Configure(EntityTypeBuilder<Task> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasMany(t => t.CommentsList).WithOne(c => c.Task);
        builder.HasOne(t => t.User).WithMany(u => u.Tasks);
    }
}