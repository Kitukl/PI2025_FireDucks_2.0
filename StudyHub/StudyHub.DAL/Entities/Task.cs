using System.Text.Json.Serialization;

namespace StudyHub.DAL.Entities
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public DateTime CreationDate { get; set; }
        public Status Status { get; set; }

        public int UserId { get; set; }

        [JsonIgnore]
        public virtual User? User { get; set; }

        public virtual List<Comments> CommentsList { get; set; } = new();
    }
}