using System.Diagnostics;
using System.Text.Json;
using StudyHub.DAL.Entities;
using StudyHub.BLL.Models;

namespace StudyHub.BLL.Services
{
    public class ScheduleService
    {
        private readonly string parserPath;
        private readonly string scheduleFolder;

        public ScheduleService()
        {
            parserPath = Path.Combine(AppContext.BaseDirectory, "Parsers", "schedule_parser.exe");
            scheduleFolder = Path.Combine(AppContext.BaseDirectory, "Resources", "Schedules");

            if (!Directory.Exists(scheduleFolder))
                Directory.CreateDirectory(scheduleFolder);
        }

        public async Task<Schedule> ParseScheduleAsync(string pdfPath, string groupName)
        {
            var psi = new ProcessStartInfo
            {
                FileName = parserPath,
                Arguments = $"\"{pdfPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
                throw new Exception($"Помилка виконання парсера: {error}");

            if (string.IsNullOrWhiteSpace(output))
                throw new Exception("Парсер не повернув JSON!");

            // 🔹 Парсимо JSON без проміжних файлів
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, List<LessonEntry>>>>>(output, options);

            if (data == null || !data.ContainsKey(groupName))
                throw new Exception($"Не знайдено розклад для групи {groupName}");

            var schedule = new Schedule { Lessons = new List<Lesson>() };

            foreach (var (day, timeSlots) in data[groupName])
            {
                foreach (var (timeRange, lessonsList) in timeSlots)
                {
                    var parts = timeRange.Split('-');
                    if (parts.Length != 2)
                        continue;

                    if (!TimeOnly.TryParse(parts[0].Trim(), out var start))
                        continue;
                    if (!TimeOnly.TryParse(parts[1].Trim(), out var end))
                        continue;

                    var slot = new LessonSlots { Start = start, End = end, Lessons = new List<Lesson>() };

                    foreach (var lessonEntry in lessonsList)
                    {
                        var lesson = new Lesson
                        {
                            Subject = new Subject { Name = lessonEntry.Subject },
                            Type = lessonEntry.Type,
                            Room = lessonEntry.Room,
                            Day = day,
                            Lecturer = lessonEntry.Teachers.Select(ParseTeacher).ToList(),
                            LessonSlot = slot
                        };
                        schedule.Lessons.Add(lesson);
                    }
                }
            }

            return schedule;
        }

        private Lecturer ParseTeacher(string full)
        {
            var parts = full.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return new Lecturer
                {
                    Surname = parts[0].Replace(",", ""),
                    Name = string.Join(' ', parts.Skip(1))
                };
            }
            return new Lecturer { Name = full };
        }
    }
}