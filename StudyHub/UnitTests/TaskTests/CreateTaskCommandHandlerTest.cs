using Moq;
using StudyHub.BLL.Commands.Task.Create;
using StudyHub.DAL.Repositories;
using StudyHub.DAL.Entities;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;
using Status = StudyHub.DAL.Entities.Status;

public class CreateCommandHandlerTest
{
    private readonly Mock<IBaseRepository<StudyHub.DAL.Entities.Task>> _mockTaskRepository;
    private readonly CreateCommandHandler _handler;

    private const int UserId = 42;
    private const int ExpectedTaskId = 100;
    private readonly DateTime Deadline = DateTime.UtcNow.AddDays(7);

    public CreateCommandHandlerTest()
    {
        _mockTaskRepository = new Mock<IBaseRepository<StudyHub.DAL.Entities.Task>>();
        _handler = new CreateCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CreatesTaskAndReturnsId()
    {
        // Arrange
        var command = new CreateCommand(
            UserId: UserId,
            Title: "New Test Task",
            Description: "Task description.",
            Deadline: Deadline,
            Status: Status.InProgress
        );

        StudyHub.DAL.Entities.Task capturedTask = null;

        _mockTaskRepository
            .Setup(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .Callback<StudyHub.DAL.Entities.Task>(task =>
            {
                capturedTask = task;
                task.Id = ExpectedTaskId;
            })
            .ReturnsAsync(() => capturedTask);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), Times.Once);

        Assert.Equal(ExpectedTaskId, resultId);
        Assert.NotNull(capturedTask);
        Assert.Equal(command.Title, capturedTask.Title);
        Assert.Equal(command.Description, capturedTask.Description);
        Assert.Equal(command.Deadline.ToUniversalTime(), capturedTask.Deadline);
        Assert.Equal(command.Status, capturedTask.Status);
        Assert.Equal(UserId, capturedTask.UserId);

        Assert.True(capturedTask.CreationDate <= DateTime.UtcNow);
        Assert.True(capturedTask.CreationDate > DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_InvalidUserId_StillCreatesTask()
    {
        // Arrange
        var command = new CreateCommand(
            UserId: 0,
            Title: "Task Title",
            Description: "Desc",
            Deadline: Deadline,
            Status: Status.Done
        );

        StudyHub.DAL.Entities.Task capturedTask = null;

        _mockTaskRepository
            .Setup(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .Callback<StudyHub.DAL.Entities.Task>(task =>
            {
                capturedTask = task;
                task.Id = 50;
            })
            .ReturnsAsync(() => capturedTask);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(50, resultId);
        _mockTaskRepository.Verify(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), Times.Once);
    }
}