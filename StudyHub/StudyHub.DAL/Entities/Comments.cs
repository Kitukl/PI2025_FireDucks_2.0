using System.Text.Json.Serialization;

namespace StudyHub.DAL.Entities;

public class Comments
{
    public int Id { get; set; }
    public string Description { get; set; }
    public DateTime CreationDate { get; set; }
    [JsonIgnore]
    public Task Task { get; set; }
}