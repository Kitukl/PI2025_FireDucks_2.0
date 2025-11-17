using Moq;
using StudyHub.BLL.Commands.Task.Delete;
using StudyHub.DAL.Repositories;
using Xunit;
using System.Threading;
using System.Threading.Tasks;

public class DeleteCommandHandlerTests
{
    private readonly Mock<TaskRepository> _mockTaskRepository;
    private readonly DeleteCommandHandler _handler;

    private const int TaskIdToDelete = 15;

    public DeleteCommandHandlerTests()
    {
        _mockTaskRepository = new Mock<TaskRepository>(new object[] { null });
        _handler = new DeleteCommandHandler(_mockTaskRepository.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskExists_ReturnsDeletedTaskId()
    {
        // Arrange
        var command = new DeleteCommand(TaskIdToDelete);
        
        _mockTaskRepository
            .Setup(r => r.DeleteAsync(TaskIdToDelete))
            .ReturnsAsync(TaskIdToDelete);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(r => r.DeleteAsync(TaskIdToDelete), Times.Once);
        Assert.Equal(TaskIdToDelete, resultId);
    }

    [Fact]
    public async System.Threading.Tasks.Task Handle_TaskDoesNotExist_ReturnsIdAndCallsRepository()
    {
        // Arrange
        const int nonExistentTaskId = 999;
        var command = new DeleteCommand(nonExistentTaskId);
        _mockTaskRepository
            .Setup(r => r.DeleteAsync(nonExistentTaskId))
            .ReturnsAsync(nonExistentTaskId);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockTaskRepository.Verify(r => r.DeleteAsync(nonExistentTaskId), Times.Once);
        Assert.Equal(nonExistentTaskId, resultId);
    }
}