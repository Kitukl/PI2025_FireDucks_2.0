using Moq;
using StudyHub.BLL.Commands.Comments.Create;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.UnitTests;

public class CreateCommentsCommandHandlerTests
{
    private readonly Mock<CommentsRepository> _mockCommentsRepository;
    private readonly Mock<TaskRepository> _mockTaskRepository;

    private readonly CreateCommandHandler _handler;

    private const int ExpectedCommentId = 200;
    private const int TestTaskId = 101;
    private readonly StudyHub.DAL.Entities.Task _testTask = new StudyHub.DAL.Entities.Task { Id = TestTaskId, Title = "Test Task" };

    public CreateCommentsCommandHandlerTests()
    {
        _mockCommentsRepository = new Mock<CommentsRepository>(new object[] { null });
        _mockTaskRepository = new Mock<TaskRepository>(new object[] { null });

        _handler = new CreateCommandHandler(_mockCommentsRepository.Object, _mockTaskRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommandAndTaskExists_CreatesCommentAndReturnsId()
    {
        // Arrange
        var command = new CreateCommand(
            taskId: TestTaskId,
            description: "New Test Comment."
        );
        
        Comments capturedComment = null;
        _mockTaskRepository
            .Setup(r => r.GetById(TestTaskId))
            .ReturnsAsync(_testTask);

        _mockCommentsRepository
            .Setup(r => r.CreateAsync(It.IsAny<Comments>()))
            .Callback<Comments>(comment =>
            {
                capturedComment = comment;
                comment.Id = ExpectedCommentId;
            })
            .ReturnsAsync(() => capturedComment); 

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert

        _mockTaskRepository.Verify(r => r.GetById(TestTaskId), Times.Once);
        _mockCommentsRepository.Verify(r => r.CreateAsync(It.IsAny<Comments>()), Times.Once);

        Assert.Equal(ExpectedCommentId, resultId);

        Assert.NotNull(capturedComment);
        Assert.Equal(command.description, capturedComment.Description);
        Assert.Equal(_testTask, capturedComment.Task);

        Assert.True(capturedComment.CreationDate <= DateTime.UtcNow);
        Assert.True(capturedComment.CreationDate > DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(5)));
    }

    [Fact]
    public async Task Handle_TaskNotFound_ThrowsException()
    {
        // Arrange
        const int NonExistentTaskId = 999;
        var command = new CreateCommand(
            taskId: NonExistentTaskId,
            description: "Comment"
        );
    
        _mockTaskRepository
            .Setup(r => r.GetById(NonExistentTaskId))
            .ReturnsAsync((StudyHub.DAL.Entities.Task)null);
    
        // Act & Assert
        
        await Assert.ThrowsAsync<InvalidOperationException>( 
            () => _handler.Handle(command, CancellationToken.None)
        );
    
        // Перевіряємо, що створення коментаря не було викликано
        _mockCommentsRepository.Verify(r => r.CreateAsync(It.IsAny<Comments>()), Times.Never);
        _mockTaskRepository.Verify(r => r.GetById(NonExistentTaskId), Times.Once);
    }
}