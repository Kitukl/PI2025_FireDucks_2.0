using System.Text.Json.Serialization;

namespace StudyHub.DAL.Entities;

public class Task
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime CreationDate { get; set; }

    public Status Status { get; set; }

    [JsonIgnore]
    public User User { get; set; }

    public List<Comments> CommentsList { get; set; }
}