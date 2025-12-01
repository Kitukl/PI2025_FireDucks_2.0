using StudyHub.BLL.Models;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using System.Diagnostics;
using System.Globalization;
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
            var allGroupsData = await RunExternalParserAsync(pdfPath);

            var (lecturerCache, subjectCache, slotCache) = await LoadCachesAsync();
            var existingSchedules = await _scheduleRepo.GetAll();

            Schedule? resultSchedule = null;

            foreach (var groupName in allGroupsData.Keys)
            {
                var currentSchedule = await ProcessSingleGroupAsync(
                    groupName,
                    allGroupsData[groupName],
                    existingSchedules,
                    lecturerCache,
                    subjectCache,
                    slotCache
                );

                await SaveScheduleToDbAsync(currentSchedule, existingSchedules.Any(s => s.GroupName == groupName));

                if (groupName.Equals(requestedGroupName, StringComparison.OrdinalIgnoreCase))
                {
                    resultSchedule = currentSchedule;
                }
            }

            return await GetFinalResultAsync(resultSchedule, requestedGroupName);
        }


        private async Task<Dictionary<string, Dictionary<string, Dictionary<string, List<LessonEntry>>>>> RunExternalParserAsync(string pdfPath)
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
                throw new InvalidOperationException($"Помилка виконання парсера: {error}");

            if (string.IsNullOrWhiteSpace(output))
                throw new InvalidDataException("Парсер не повернув JSON!");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, Dictionary<string, List<LessonEntry>>>>>(output, options);

            if (data == null || data.Count == 0)
                throw new KeyNotFoundException("У файлі не знайдено даних розкладу.");

            return data;
        }

        private async Task<(Dictionary<string, Lecturer> Lecturers, Dictionary<string, Subject> Subjects, Dictionary<string, LessonSlots> Slots)> LoadCachesAsync()
        {
            var lecturers = (await _lecturerRepo.GetAll())
                .GroupBy(l => $"{l.Surname} {l.Name}".Trim())
                .ToDictionary(g => g.Key, g => g.First());

            var subjects = (await _subjectRepo.GetAll())
                .GroupBy(s => s.Name.Trim())
                .ToDictionary(g => g.Key, g => g.First());

            var slots = (await _lessonSlotsRepo.GetAll())
                .ToDictionary(ls => $"{ls.Start:HH:mm}-{ls.End:HH:mm}", ls => ls);

            return (lecturers, subjects, slots);
        }

        private async Task<Schedule> ProcessSingleGroupAsync(
            string groupName,
            Dictionary<string, Dictionary<string, List<LessonEntry>>> groupData,
            List<Schedule> existingSchedules,
            Dictionary<string, Lecturer> lecturerCache,
            Dictionary<string, Subject> subjectCache,
            Dictionary<string, LessonSlots> slotCache)
        {
            var schedule = PrepareScheduleEntity(groupName, existingSchedules);

            foreach (var (day, timeSlots) in groupData)
            {
                foreach (var (timeRange, lessonsList) in timeSlots)
                {
                    await ProcessTimeSlotAsync(schedule, day, timeRange, lessonsList, lecturerCache, subjectCache, slotCache);
                }
            }

            return schedule;
        }

        private Schedule PrepareScheduleEntity(string groupName, List<Schedule> existingSchedules)
        {
            var currentSchedule = existingSchedules.FirstOrDefault(s => s.GroupName == groupName);

            if (currentSchedule == null)
            {
                return new Schedule
                {
                    GroupName = groupName,
                    Lessons = new List<Lesson>()
                };
            }

            currentSchedule.Lessons.Clear();
            return currentSchedule;
        }

        private async System.Threading.Tasks.Task ProcessTimeSlotAsync(
            Schedule schedule,
            string day,
            string timeRange,
            List<LessonEntry> lessonsList,
            Dictionary<string, Lecturer> lecturerCache,
            Dictionary<string, Subject> subjectCache,
            Dictionary<string, LessonSlots> slotCache)
        {
            var parts = timeRange.Split('-');
            if (parts.Length != 2) return;

            if (!TimeOnly.TryParse(parts[0].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var start) ||
                !TimeOnly.TryParse(parts[1].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.None, out var end)) return;

            var slotKey = $"{start:HH:mm}-{end:HH:mm}";
            if (!slotCache.TryGetValue(slotKey, out var slot))
            {
                slot = new LessonSlots { Start = start, End = end };
                slotCache.Add(slotKey, slot);
            }

            foreach (var lessonEntry in lessonsList)
            {
                var lesson = await CreateLessonAsync(lessonEntry, day, slot, lecturerCache, subjectCache);
                schedule.Lessons.Add(lesson);
            }
        }

        private async Task<Lesson> CreateLessonAsync(
            LessonEntry entry,
            string day,
            LessonSlots slot,
            Dictionary<string, Lecturer> lecturerCache,
            Dictionary<string, Subject> subjectCache)
        {
            string subjectName = entry.Subject.Trim();
            if (string.IsNullOrWhiteSpace(subjectName)) subjectName = "Невідома дисципліна";

            if (!subjectCache.TryGetValue(subjectName, out var subject))
            {
                subject = new Subject { Name = subjectName };
                subjectCache.Add(subjectName, subject);
            }

            var lessonLecturers = new List<Lecturer>();
            foreach (var teacherRawName in entry.Teachers)
            {
                if (string.IsNullOrWhiteSpace(teacherRawName)) continue;

                var parsedLecturer = ParseTeacher(teacherRawName);
                if (parsedLecturer == null) continue;

                string lecturerKey = $"{parsedLecturer.Surname} {parsedLecturer.Name}".Trim();

                if (!lecturerCache.TryGetValue(lecturerKey, out var lecturer))
                {
                    lecturer = parsedLecturer;
                    lecturerCache.Add(lecturerKey, lecturer);
                }
                lessonLecturers.Add(lecturer);
            }

            return new Lesson
            {
                Subject = subject,
                Type = entry.Type,
                Room = entry.Room,
                Day = day,
                Lecturer = lessonLecturers,
                LessonSlot = slot
            };
        }

        private async System.Threading.Tasks.Task SaveScheduleToDbAsync(Schedule schedule, bool existsInDb)
        {
            try
            {
                if (existsInDb)
                {
                    await _scheduleRepo.UpdateAsync(schedule);
                }
                else
                {
                    await _scheduleRepo.CreateAsync(schedule);
                }
            }
            catch (Exception ex)
            {
                var inner = ex.InnerException;
                while (inner?.InnerException != null) inner = inner.InnerException;
                throw new InvalidOperationException($"Помилка збереження групи {schedule.GroupName}: {inner?.Message ?? ex.Message}");
            }
        }

        private async Task<Schedule> GetFinalResultAsync(Schedule? resultSchedule, string requestedGroupName)
        {
            if (resultSchedule != null) return resultSchedule;

            var freshSchedules = await _scheduleRepo.GetAll();
            resultSchedule = freshSchedules.FirstOrDefault(s => s.GroupName == requestedGroupName);

            if (resultSchedule == null)
                throw new KeyNotFoundException($"У файлі не знайдено розкладу для групи '{requestedGroupName}'.");

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