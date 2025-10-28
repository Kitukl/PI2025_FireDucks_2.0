using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class LecturerConfiguration : IEntityTypeConfiguration<Lecturer>
{
    public void Configure(EntityTypeBuilder<Lecturer> builder)
    {
        builder.HasKey(l => l.Id);
        builder.HasMany(l => l.Lessons).WithMany(l => l.Lecturer);
    }
}