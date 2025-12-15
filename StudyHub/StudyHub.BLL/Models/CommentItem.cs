using System;

namespace StudyHub.BLL.Models
{
    public class CommentItem
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public int TaskId { get; set; }
        public int UserId { get; set; }

        public string AuthorName { get; set; } = "Unknown User";
    }
}