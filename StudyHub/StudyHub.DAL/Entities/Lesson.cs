namespace StudyHub.DAL.Entities;

public class Lesson
{
    public int Id { get; set; }

    public List<Lecturer> Lecturer { get; set; }

    public LessonSlots LessonSlot { get; set; }

    public Subject Subject { get; set; }

    public List<Schedule> Schedules { get; set; }

    public string Type { get; set; } 

    public string Room { get; set; }

    public string Day { get; set; } 

}