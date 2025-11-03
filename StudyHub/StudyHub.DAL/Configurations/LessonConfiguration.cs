using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasMany(l => l.Lecturer).WithMany(l => l.Lessons);
        builder.HasOne(l => l.LessonSlot).WithMany(ls => ls.Lessons);
        builder.HasOne(l => l.Subject).WithMany(s => s.Lessons);
        builder.HasMany(l => l.Schedules).WithMany(s => s.Lessons);
    }
}