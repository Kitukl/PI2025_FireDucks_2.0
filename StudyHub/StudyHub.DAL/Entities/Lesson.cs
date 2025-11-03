namespace StudyHub.DAL.Entities;

public class Lesson
{
    public int Id { get; set; }
    public List<Lecturer> Lecturer { get; set; }
    public LessonSlots LessonSlot { get; set; }
    public Subject Subject { get; set; }
    public List<Schedule> Schedules { get; set; }

    // додав в цю сутність три нові необхідні для парсера поля, їх немає в UML діаграмі, тому треба буде додати
    public string Type { get; set; } // лекція / лаб. / практ.
    public string Room { get; set; } // аудиторія
    public string Day { get; set; }  // день тижня

}