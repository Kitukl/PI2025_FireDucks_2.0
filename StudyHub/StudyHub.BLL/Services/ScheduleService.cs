using StudyHub.BLL.Models;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using System.Diagnostics;
using System.Text.Json;

namespace StudyHub.BLL.Services
{
    public class ScheduleService
    {
        private readonly string parserPath;
        private readonly string scheduleFolder;
        private readonly IBaseRepository<Lecturer> _lecturerRepo;
        private readonly IBaseRepository<Subject> _subjectRepo;
        private readonly IBaseRepository<LessonSlots> _lessonSlotsRepo;
        private readonly IBaseRepository<Schedule> _scheduleRepo;

        public ScheduleService(
            IBaseRepository<Lecturer> lecturerRepo,
            IBaseRepository<Subject> subjectRepo,
            IBaseRepository<LessonSlots> lessonSlotsRepo,
            IBaseRepository<Schedule> scheduleRepo)
        {
            _lecturerRepo = lecturerRepo;
            _subjectRepo = subjectRepo;
            _lessonSlotsRepo = lessonSlotsRepo;
            _scheduleRepo = scheduleRepo;

            parserPath = Path.Combine(AppContext.BaseDirectory, "Parsers", "schedule_parser.exe");
            scheduleFolder = Path.Combine(AppContext.BaseDirectory, "Resources", "Schedules");

            if (!Directory.Exists(scheduleFolder))
                Directory.CreateDirectory(scheduleFolder);
        }

        public async Task<Schedule> ParseScheduleAsync(string pdfPath, string requestedGroupName)
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

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var allGroupsData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, List<LessonEntry>>>>>(output, options);

            if (allGroupsData == null || allGroupsData.Count == 0)
                throw new Exception("У файлі не знайдено даних розкладу.");

            var allLecturers = (await _lecturerRepo.GetAll())
                .GroupBy(l => $"{l.Surname} {l.Name}".Trim())
                .ToDictionary(g => g.Key, g => g.First());

            var allSubjects = (await _subjectRepo.GetAll())
                .GroupBy(s => s.Name.Trim())
                .ToDictionary(g => g.Key, g => g.First());

            var allSlots = (await _lessonSlotsRepo.GetAll())
                .ToDictionary(ls => $"{ls.Start:HH:mm}-{ls.End:HH:mm}", ls => ls);

            var existingSchedules = await _scheduleRepo.GetAll();

            Schedule? resultSchedule = null;

            foreach (var groupName in allGroupsData.Keys)
            {
                var groupScheduleData = allGroupsData[groupName];

                var currentSchedule = existingSchedules.FirstOrDefault(s => s.GroupName == groupName);
                bool isNew = false;

                if (currentSchedule == null)
                {
                    isNew = true;
                    currentSchedule = new Schedule
                    {
                        GroupName = groupName,
                        Lessons = new List<Lesson>()
                    };
                }
                else
                {
                    currentSchedule.Lessons.Clear();
                }

                foreach (var (day, timeSlots) in groupScheduleData)
                {
                    foreach (var (timeRange, lessonsList) in timeSlots)
                    {
                        var parts = timeRange.Split('-');
                        if (parts.Length != 2) continue;
                        if (!TimeOnly.TryParse(parts[0].Trim(), out var start)) continue;
                        if (!TimeOnly.TryParse(parts[1].Trim(), out var end)) continue;

                        var slotKey = $"{start:HH:mm}-{end:HH:mm}";

                        if (!allSlots.TryGetValue(slotKey, out var slot))
                        {
                            slot = new LessonSlots { Start = start, End = end };
                            allSlots.Add(slotKey, slot);
                        }

                        foreach (var lessonEntry in lessonsList)
                        {
                            string subjectName = lessonEntry.Subject.Trim();
                            if (string.IsNullOrWhiteSpace(subjectName)) subjectName = "Невідома дисципліна";

                            if (!allSubjects.TryGetValue(subjectName, out var subject))
                            {
                                subject = new Subject { Name = subjectName };
                                allSubjects.Add(subjectName, subject);
                            }

                            var lessonLecturers = new List<Lecturer>();
                            foreach (var teacherRawName in lessonEntry.Teachers)
                            {
                                if (string.IsNullOrWhiteSpace(teacherRawName)) continue;

                                var parsedLecturer = ParseTeacher(teacherRawName);

                                if (parsedLecturer == null) continue;

                                string lecturerKey = $"{parsedLecturer.Surname} {parsedLecturer.Name}".Trim();

                                if (!allLecturers.TryGetValue(lecturerKey, out var lecturer))
                                {
                                    lecturer = parsedLecturer;
                                    allLecturers.Add(lecturerKey, lecturer);
                                }
                                lessonLecturers.Add(lecturer);
                            }

                            var lesson = new Lesson
                            {
                                Subject = subject,
                                Type = lessonEntry.Type,
                                Room = lessonEntry.Room,
                                Day = day,
                                Lecturer = lessonLecturers,
                                LessonSlot = slot
                            };

                            currentSchedule.Lessons.Add(lesson);
                        }
                    }
                }

                try
                {
                    if (isNew)
                    {
                        await _scheduleRepo.CreateAsync(currentSchedule);
                    }
                    else
                    {
                        await _scheduleRepo.UpdateAsync(currentSchedule);
                    }
                }
                catch (Exception ex)
                {
                    var inner = ex.InnerException;
                    while (inner?.InnerException != null) inner = inner.InnerException;
                    throw new Exception($"Помилка збереження групи {groupName}: {inner?.Message ?? ex.Message}");
                }

                if (groupName == requestedGroupName)
                {
                    resultSchedule = currentSchedule;
                }
            }

            if (resultSchedule == null)
            {
                var freshSchedules = await _scheduleRepo.GetAll();
                resultSchedule = freshSchedules.FirstOrDefault(s => s.GroupName == requestedGroupName);

                if (resultSchedule == null)
                    throw new Exception($"У файлі не знайдено розкладу для групи '{requestedGroupName}'.");
            }

            return resultSchedule;
        }

        private Lecturer ParseTeacher(string full)
        {
            if (string.IsNullOrWhiteSpace(full)) return null;

            var parts = full.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length >= 2)
            {
                return new Lecturer
                {
                    Surname = parts[0].Replace(",", "").Trim(),
                    Name = string.Join(' ', parts.Skip(1)).Trim()
                };
            }

            return new Lecturer
            {
                Surname = full.Replace(",", "").Trim(),
                Name = string.Empty
            };
        }
    }
}