using Moq;
using StudyHub.BLL.Commands.Comments.Delete;
using StudyHub.DAL.Repositories;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;

// Припускаємо, що DeleteCommand виглядає так:
// public record DeleteCommand(int id) : IRequest<int>;

public class DeleteCommentsCommandHandlerTests
{
    private readonly Mock<CommentsRepository> _mockCommentsRepository;
    private readonly DeleteCommandHandler _handler;

    private const int ExistingCommentId = 150;
    private const int NonExistingCommentId = 999;
    
    public DeleteCommentsCommandHandlerTests()
    {
        _mockCommentsRepository = new Mock<CommentsRepository>(new object[] { null });

        _handler = new DeleteCommandHandler(_mockCommentsRepository.Object);
    }

    [Fact]
    public async Task Handle_ExistingCommentId_ReturnsOneAndDeletes()
    {
        // Arrange
        var command = new DeleteCommand(id: ExistingCommentId);
        const int expectedRowsAffected = 1;

        _mockCommentsRepository
            .Setup(r => r.DeleteAsync(ExistingCommentId))
            .ReturnsAsync(expectedRowsAffected);

        // Act
        var resultRows = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCommentsRepository.Verify(r => r.DeleteAsync(ExistingCommentId), Times.Once);
        Assert.Equal(expectedRowsAffected, resultRows);
    }

    [Fact]
    public async Task Handle_NonExistingCommentId_ReturnsZeroAndAttemptsDelete()
    {
        // Arrange
        var command = new DeleteCommand(id: NonExistingCommentId);
        const int expectedRowsAffected = 0; 

        _mockCommentsRepository
            .Setup(r => r.DeleteAsync(NonExistingCommentId))
            .ReturnsAsync(expectedRowsAffected);

        // Act
        var resultRows = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockCommentsRepository.Verify(r => r.DeleteAsync(NonExistingCommentId), Times.Once);

        Assert.Equal(expectedRowsAffected, resultRows);
    }
    
    [Fact]
    public async Task Handle_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var command = new DeleteCommand(id: ExistingCommentId);
        var expectedException = new InvalidOperationException("Simulated DB error.");

        _mockCommentsRepository
            .Setup(r => r.DeleteAsync(ExistingCommentId))
            .ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );
        _mockCommentsRepository.Verify(r => r.DeleteAsync(ExistingCommentId), Times.Once);
    }
}