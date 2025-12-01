namespace StudyHub.DAL.Entities;

public class Schedule
{
    public int Id { get; set; }
    public string GroupName { get; set; } = String.Empty;
    public List<Lesson> Lessons { get; set; }

    public List<User> Users { get; set; }

}