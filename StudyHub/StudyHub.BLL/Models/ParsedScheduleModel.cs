using System.Collections.Generic;

namespace StudyHub.BLL.Models
{
    public class ParsedScheduleModel
    {
        public Dictionary<string, GroupSchedule> Groups { get; set; } = new();
    }

    public class GroupSchedule
    {
        public Dictionary<string, Dictionary<string, List<LessonEntry>>> Days { get; set; } = new();
    }

    public class LessonEntry
    {
        public string Subject { get; set; } = "";
        public string Type { get; set; } = "";
        public string Room { get; set; } = "";
        public List<string> Teachers { get; set; } = new();
    }
}