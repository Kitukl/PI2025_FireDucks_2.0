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
    private readonly Mock<TaskRepository> _mockTaskRepository;
    private readonly Mock<UserRepository> _mockUserRepository;
    private readonly CreateCommandHandler _handler;

    private const int UserId = 42;
    private const int ExpectedTaskId = 100;
    private readonly DateTime Deadline = DateTime.UtcNow.AddDays(7);
    private readonly StudyHub.DAL.Entities.User _testUser = new StudyHub.DAL.Entities.User { Id = UserId, Name = "Test User" };

    public CreateCommandHandlerTest()
    {
        _mockTaskRepository = new Mock<TaskRepository>(new object[] { null });
        _mockUserRepository = new Mock<UserRepository>(new object[] { null });
        
        _handler = new CreateCommandHandler(_mockTaskRepository.Object, _mockUserRepository.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommandAndUserExists_CreatesTaskAndReturnsId()
    {
        // Arrange
        var command = new CreateCommand(
            userId: UserId,
            title: "New Test Task",
            description: "Task description.",
            deadline: Deadline,
            status: Status.InProgress
        );
        StudyHub.DAL.Entities.Task capturedTask = null;

        _mockUserRepository
            .Setup(r => r.GetById(UserId))
            .ReturnsAsync(_testUser);

        _mockTaskRepository
            .Setup(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .Callback<StudyHub.DAL.Entities.Task>(task =>
            {
                capturedTask = task;
                task.Id = ExpectedTaskId;
            })
            // ✅ Вказуємо тип повернення як StudyHub.DAL.Entities.Task
            .ReturnsAsync(() => capturedTask);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(r => r.GetById(UserId), Times.Once);
        _mockTaskRepository.Verify(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), Times.Once);

        Assert.Equal(ExpectedTaskId, resultId);

        Assert.NotNull(capturedTask);
        Assert.Equal(command.title, capturedTask.Title);
        Assert.Equal(command.description, capturedTask.Description);
        Assert.Equal(command.deadline, capturedTask.Deadline);
        Assert.Equal(command.status, capturedTask.Status);
        Assert.Equal(_testUser, capturedTask.User);
        
        Assert.True(capturedTask.CreationDate <= DateTime.Now);
        Assert.True(capturedTask.CreationDate > DateTime.Now.Subtract(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_UserNotFound_ThrowsNullReferenceException()
    {
        // Arrange
        var command = new CreateCommand(
            userId: 999,
            title: "Task Title",
            description: "Desc",
            deadline: Deadline,
            status: Status.Done
        );

        _mockUserRepository
            .Setup(r => r.GetById(999))
            .ReturnsAsync((StudyHub.DAL.Entities.User)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        _mockTaskRepository.Verify(r => r.CreateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), Times.Never);
        _mockUserRepository.Verify(r => r.GetById(999), Times.Once);
    }
}