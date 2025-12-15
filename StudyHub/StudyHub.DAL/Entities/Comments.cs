using System.Text.Json.Serialization;

namespace StudyHub.DAL.Entities;

public class Comments
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }

    public int TaskId { get; set; }
    public int UserId { get; set; }

    [JsonIgnore]
    public Task Task { get; set; }

    [JsonIgnore]
    public User User { get; set; }
}