namespace StudyHub.DAL.Entities;

public class SupportTicket
{
    public int Id { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public Category Category { get; set; }
    public User User { get; set; }
}