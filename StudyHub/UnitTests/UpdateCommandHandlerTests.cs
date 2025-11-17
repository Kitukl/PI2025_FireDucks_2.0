using Moq;
using StudyHub.BLL.Commands.Task.Update;
using StudyHub.DAL.Repositories;
using StudyHub.DAL.Entities;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;

public class UpdateCommandHandlerTests
{
    private readonly Mock<TaskRepository> _mockTaskRepository;
    private readonly UpdateCommandHandler _handler;

    private const int TaskIdToUpdate = 10;
    private const string NewTitle = "Updated Task Title";
    private readonly StudyHub.DAL.Entities.Task _updatedTask = new StudyHub.DAL.Entities.Task
    {
        Id = TaskIdToUpdate,
        Title = NewTitle,
        Description = "New Description",
        Deadline = DateTime.UtcNow.AddDays(10),
        Status = Status.InProgress,
        CreationDate = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
        User = new StudyHub.DAL.Entities.User { Id = 1 }
    };

    public UpdateCommandHandlerTests()
    {
        _mockTaskRepository = new Mock<TaskRepository>(new object[] { null });
        _handler = new UpdateCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_ValidCommand_CallsRepositoryUpdateAndReturnsId()
    {
        // Arrange
        var command = new UpdateCommand(_updatedTask);
        _mockTaskRepository
            .Setup(r => r.UpdateAsync(It.Is<StudyHub.DAL.Entities.Task>(t => t.Id == TaskIdToUpdate)))
            .ReturnsAsync(TaskIdToUpdate);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(
            r => r.UpdateAsync(_updatedTask), 
            Times.Once
        );
        Assert.Equal(TaskIdToUpdate, resultId);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_RepositoryThrowsException_ThrowsExceptionUpwards()
    {
        // Arrange
        var command = new UpdateCommand(_updatedTask);
        var expectedException = new Exception("Database error occurred.");

        _mockTaskRepository
            .Setup(r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var actualException = await Assert.ThrowsAsync<Exception>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        Assert.Equal(expectedException, actualException);
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), Times.Once);
    }
}