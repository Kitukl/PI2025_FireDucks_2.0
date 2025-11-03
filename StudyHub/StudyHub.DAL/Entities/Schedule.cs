namespace StudyHub.DAL.Entities;

public class Schedule
{
    public int Id { get; set; }
    public List<Lesson> Lessons { get; set; }
    public List<User> Users { get; set; }
}