namespace StudyHub.DAL.Entities;

public class LessonSlots
{
    public int Id { get; set; }
    public TimeOnly Start { get; set; }
    public TimeOnly End { get; set; }
    public List<Lesson> Lessons { get; set; }
}