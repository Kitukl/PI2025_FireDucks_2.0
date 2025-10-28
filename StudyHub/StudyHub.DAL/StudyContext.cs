using Microsoft.EntityFrameworkCore;
using StudyHub.DAL.Entities;
using Task = StudyHub.DAL.Entities.Task;

namespace StudyHub.DAL;

public class StudyContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<SupportTicket> Tickets { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<LessonSlots> LessonSlots { get; set; }
    public DbSet<Lesson> Lessons { get; set; }
    public DbSet<Lecturer> Lecturers { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StudyContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=aws-1-eu-west-1.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.ddlufspllptpsrlfzvvj;Password=Pa33word-;SslMode=Require;Trust Server Certificate=True;Pooling=true;");
    }
}