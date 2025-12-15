using Moq;
using StudyHub.BLL.Commands.User.Login;
using StudyHub.DAL.Repositories;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using System;
using StudyHub.DAL.Entities;
using BCrypt.Net;
using Task = System.Threading.Tasks.Task;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserRepository> _mockUserRepository;
    private readonly LoginCommandHandler _handler;

    private readonly StudyHub.DAL.Entities.User _testUser = new StudyHub.DAL.Entities.User
    {
        Id = 1,
        Name = "Тест",
        Surname = "Користувач",
        Email = "test@example.com",
        Password = BCrypt.Net.BCrypt.HashPassword("ValidPassword123")
    };

    public LoginCommandHandlerTests()
    {
        _mockUserRepository = new Mock<UserRepository>(new object[] { null });
        _handler = new LoginCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsUserLoginResponse()
    {
        // Arrange
        const string validEmail = "test@example.com";
        const string validPassword = "ValidPassword123";
        var command = new LoginCommand(validEmail, validPassword);

        _mockUserRepository
            .Setup(r => r.GetByEmail(validEmail))
            .ReturnsAsync(_testUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(r => r.GetByEmail(validEmail), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(_testUser.Id, result.Id);
        Assert.Equal(_testUser.Email, result.Email);
        Assert.Equal(_testUser.Name, result.Name);
        Assert.Equal(_testUser.Surname, result.Surname);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        const string nonExistentEmail = "missing@example.com";
        var command = new LoginCommand(nonExistentEmail, "AnyPassword");

        _mockUserRepository
            .Setup(r => r.GetByEmail(nonExistentEmail))
            .ThrowsAsync(new Exception("Not found user"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Користувача з таким email не знайдено", exception.Message);
        _mockUserRepository.Verify(r => r.GetByEmail(nonExistentEmail), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        const string email = "test@example.com";
        const string invalidPassword = "WrongPassword";
        var command = new LoginCommand(email, invalidPassword);

        _mockUserRepository
            .Setup(r => r.GetByEmail(email))
            .ReturnsAsync(_testUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        Assert.Equal("Неправильний пароль", exception.Message);
        _mockUserRepository.Verify(r => r.GetByEmail(email), Times.Once);
    }
}