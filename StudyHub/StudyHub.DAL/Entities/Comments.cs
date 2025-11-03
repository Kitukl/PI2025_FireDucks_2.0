namespace StudyHub.DAL.Entities;

public class Comments
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    public Task Task { get; set; }
}