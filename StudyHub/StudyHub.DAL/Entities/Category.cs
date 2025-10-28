namespace StudyHub.DAL.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<SupportTicket> Tickets { get; set; }
}