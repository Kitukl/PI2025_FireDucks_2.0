using Moq;
using StudyHub.BLL.Commands.User.Register;
using StudyHub.DAL.Repositories;
using StudyHub.DAL.Entities;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using BCrypt.Net;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;

public class RegisterCommandHandlerTests
{
    private readonly Mock<IBaseRepository<User>> _mockUserRepository;
    private readonly RegisterCommandHandler _handler;

    private const string Name = "John";
    private const string Surname = "Doe";
    private const string Email = "john.doe@example.com";
    private const string Password = "SecurePassword123";
    private const string GroupName = "PMI-31";
    private const int ExpectedUserId = 5;

    public RegisterCommandHandlerTests()
    {
        _mockUserRepository = new Mock<IBaseRepository<User>>();
        _handler = new RegisterCommandHandler(_mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_HashesPasswordAndCreatesUser()
    {
        // Arrange
        var command = new RegisterCommand(Name, Surname, Email, Password, GroupName);

        User capturedUser = null;

        _mockUserRepository
            .Setup(r => r.CreateAsync(It.IsAny<User>()))
            .Callback<User>(user =>
            {
                capturedUser = user;
                user.Id = ExpectedUserId;
            })
            .ReturnsAsync(() => capturedUser);

        // Act
        var resultId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _mockUserRepository.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);

        Assert.Equal(ExpectedUserId, resultId);
        Assert.NotNull(capturedUser);
        Assert.Equal(Name, capturedUser.Name);
        Assert.Equal(Surname, capturedUser.Surname);
        Assert.Equal(Email, capturedUser.Email);
        Assert.Equal(GroupName, capturedUser.GroupName);

        // Перевірка хешування пароля
        Assert.NotNull(capturedUser.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify(Password, capturedUser.Password));
        Assert.NotEqual(Password, capturedUser.Password);

        // Перевірка значень за замовчуванням
        Assert.Equal(7, capturedUser.DaysForNotification);
        Assert.False(capturedUser.IsNotified);
        Assert.Equal(string.Empty, capturedUser.UserPhoto);
        Assert.NotNull(capturedUser.Schedule);
        Assert.Empty(capturedUser.Schedule);
        Assert.NotNull(capturedUser.Tasks);
        Assert.Empty(capturedUser.Tasks);
        Assert.NotNull(capturedUser.Tickets);
        Assert.Empty(capturedUser.Tickets);
    }
}