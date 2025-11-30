using Moq;
using StudyHub.BLL.Commands.Task.Update;
using StudyHub.DAL.Repositories;
using StudyHub.DAL.Entities;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;
using Status = StudyHub.DAL.Entities.Status;
using Task = System.Threading.Tasks.Task; // Припускаємо, що у вас є enum Status

public class UpdateCommandHandlerTest
{
    private readonly Mock<TaskRepository> _mockTaskRepository;
    private readonly UpdateCommandHandler _handler;

    private const int TaskIdToUpdate = 105;
    private const int ExpectedRowsAffected = 1;
    
    private readonly StudyHub.DAL.Entities.Task _taskToUpdate = new StudyHub.DAL.Entities.Task
    {
        Id = TaskIdToUpdate,
        Title = "Original Title",
        Description = "Original Description",
        Deadline = DateTime.UtcNow.AddDays(10),
        Status = Status.InProgress 
    };

    public UpdateCommandHandlerTest()
    {
        _mockTaskRepository = new Mock<TaskRepository>(new object[] { null });
        _handler = new UpdateCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_UpdatesTaskAndReturnsOne()
    {
        // Arrange
        _taskToUpdate.Title = "Updated Title";
        _taskToUpdate.Status = Status.Done;
        
        var command = new UpdateCommand(task: _taskToUpdate);

        _mockTaskRepository
            .Setup(r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .ReturnsAsync(ExpectedRowsAffected);

        // Act
        var resultRows = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(
            r => r.UpdateAsync(It.Is<StudyHub.DAL.Entities.Task>(t => t.Id == TaskIdToUpdate)), 
            Times.Once
        );
        Assert.Equal(ExpectedRowsAffected, resultRows);
    }
    
    [Fact]
    public async Task Handle_TaskNotFoundInDb_ReturnsZero()
    {
        // Arrange
        var command = new UpdateCommand(task: _taskToUpdate);
        _mockTaskRepository
            .Setup(r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .ReturnsAsync(0);

        // Act
        var resultRows = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(
            r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), 
            Times.Once
        );

        Assert.Equal(0, resultRows);
    }

    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Assert
        var command = new UpdateCommand(task: _taskToUpdate);
        var expectedException = new InvalidOperationException("Simulated database error during update.");
        
        _mockTaskRepository
            .Setup(r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        
        _mockTaskRepository.Verify(r => r.UpdateAsync(It.IsAny<StudyHub.DAL.Entities.Task>()), Times.Once);
    }
}