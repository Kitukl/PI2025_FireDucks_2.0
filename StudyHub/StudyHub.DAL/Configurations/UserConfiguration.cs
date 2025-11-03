using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.HasMany(u => u.Schedule).WithMany(s => s.Users);
        builder.HasMany(u => u.Tasks).WithOne(t => t.User);
        builder.HasMany(u => u.Tickets).WithOne(t => t.User);
    }
}