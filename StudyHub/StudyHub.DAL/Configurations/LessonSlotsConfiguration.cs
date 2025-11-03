using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class LessonSlotsConfiguration : IEntityTypeConfiguration<LessonSlots>
{
    public void Configure(EntityTypeBuilder<LessonSlots> builder)
    {
        builder.HasKey(ls => ls.Id);
        builder.HasMany(ls => ls.Lessons).WithOne(l => l.LessonSlot);
    }
}