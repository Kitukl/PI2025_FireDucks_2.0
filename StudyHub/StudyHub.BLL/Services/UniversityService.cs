namespace StudyHub.BLL.Services;

public class UniversityService
{
    private readonly IEnumerable<IFacultyHelper> _faculties;

    public UniversityService(IEnumerable<IFacultyHelper> faculties)
    {
        _faculties = faculties;
    }

    public IFacultyHelper GetFacultyByGroup(string groupName)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            throw new ArgumentException("Назва групи не може бути порожньою");
        }

        var faculty = _faculties.FirstOrDefault(f => f.IsMatch(groupName));

        if (faculty == null)
        {
            throw new ArgumentException($"Не знайдено факультет для групи {groupName}. Перевірте правильність назви.");
        }

        return faculty;
    }
}