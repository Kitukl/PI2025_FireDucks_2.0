namespace StudyHub.DAL.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserPhoto { get; set; }
    public int DaysForNotification { get; set; }
    public bool IsNotified { get; set; }
    public List<Schedule> Schedule { get; set; }
    public List<Task> Tasks { get; set; }
    public List<SupportTicket> Tickets { get; set; }
}