using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
{
    public void Configure(EntityTypeBuilder<Schedule> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasMany(s => s.Lessons).WithMany(l => l.Schedules);
        builder.HasMany(s => s.Users).WithMany(u => u.Schedule);
    }
}