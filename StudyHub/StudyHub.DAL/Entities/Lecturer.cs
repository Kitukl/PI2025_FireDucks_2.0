namespace StudyHub.DAL.Entities;

public class Lecturer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public List<Lesson> Lessons { get; set; }

}