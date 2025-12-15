using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = StudyHub.DAL.Entities.Task;

namespace StudyHub.DAL.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Deadline)
                .IsRequired();

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.CreationDate)
                .IsRequired();

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(t => t.CommentsList)
                .WithOne(c => c.Task)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}