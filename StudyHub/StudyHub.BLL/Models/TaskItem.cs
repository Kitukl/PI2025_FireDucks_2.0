namespace StudyHub.BLL.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string Status { get; set; } = "To Do";

        public DateTime? DueDate { get; set; }

        public DateTime CreationDate { get; set; } = DateTime.Now;

        public int ReminderDays { get; set; } = 2;

        public string? Group { get; set; }

    }
}