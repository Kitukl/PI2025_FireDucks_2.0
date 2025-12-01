using Moq;
using StudyHub.BLL.Interfaces;
using StudyHub.BLL.Services;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using Xunit;
using Task = System.Threading.Tasks.Task;

public class ScheduleServiceTests
{
    private readonly Mock<IBaseRepository<Lecturer>> _lecturerRepoMock = new();
    private readonly Mock<IBaseRepository<Subject>> _subjectRepoMock = new();
    private readonly Mock<IBaseRepository<LessonSlots>> _slotsRepoMock = new();
    private readonly Mock<IBaseRepository<Schedule>> _scheduleRepoMock = new();
    private readonly Mock<IParserRunner> _parserRunnerMock = new();

    private readonly ScheduleService _service;

    public ScheduleServiceTests()
    {
        _lecturerRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Lecturer>());
        _subjectRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Subject>());
        _slotsRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<LessonSlots>());
        _scheduleRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Schedule>());

        _service = new ScheduleService(
            _lecturerRepoMock.Object,
            _subjectRepoMock.Object,
            _slotsRepoMock.Object,
            _scheduleRepoMock.Object,
            _parserRunnerMock.Object
        );
    }

    [Fact]
    public async Task ParseScheduleAsync_ShouldCreateNewSchedule_WhenNoneExists()
    {
        // Arrange
        string groupName = "TestGroup";
        string jsonOutput = GetValidJson(groupName);

        _parserRunnerMock.Setup(p => p.RunParserAsync(It.IsAny<string>()))
            .ReturnsAsync(jsonOutput);

        // Act
        var result = await _service.ParseScheduleAsync("dummy.pdf", groupName);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(groupName, result.GroupName);
        Assert.Single(result.Lessons);

        _scheduleRepoMock.Verify(r => r.CreateAsync(It.Is<Schedule>(s => s.GroupName == groupName)), Times.Once);
        _scheduleRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Schedule>()), Times.Never);
    }

    [Fact]
    public async Task ParseScheduleAsync_ShouldUpdateSchedule_WhenItExists()
    {
        // Arrange
        string groupName = "ExistingGroup";
        string jsonOutput = GetValidJson(groupName);

        var existingSchedule = new Schedule { Id = 1, GroupName = groupName, Lessons = new List<Lesson>() };

        _scheduleRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Schedule> { existingSchedule });

        _parserRunnerMock.Setup(p => p.RunParserAsync(It.IsAny<string>()))
            .ReturnsAsync(jsonOutput);

        // Act
        await _service.ParseScheduleAsync("dummy.pdf", groupName);

        // Assert
        _scheduleRepoMock.Verify(r => r.UpdateAsync(It.Is<Schedule>(s => s.Id == 1)), Times.Once);
        _scheduleRepoMock.Verify(r => r.CreateAsync(It.IsAny<Schedule>()), Times.Never);
    }

    private string GetValidJson(string groupName)
    {
        return $@"
        {{
            ""{groupName}"": {{
                ""Понеділок"": {{
                    ""8:30 - 9:50"": [
                        {{
                            ""subject"": ""Test Subject"",
                            ""type"": ""Lek"",
                            ""room"": ""101"",
                            ""teachers"": [""Test Teacher""]
                        }}
                    ]
                }}
            }}
        }}";
    }
}