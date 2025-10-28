using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StudyHub.DAL.Entities;

namespace StudyHub.DAL.Configurations;

public class TicketsConfiguration : IEntityTypeConfiguration<SupportTicket>
{
    public void Configure(EntityTypeBuilder<SupportTicket> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasOne(t => t.User).WithMany(u => u.Tickets);
        builder.HasOne(t => t.Category).WithMany(c => c.Tickets);
    }
}