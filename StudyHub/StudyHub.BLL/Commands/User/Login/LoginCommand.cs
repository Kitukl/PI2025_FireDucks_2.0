using MediatR;

namespace StudyHub.BLL.Commands.User.Login;

public record LoginCommand(string email, string password) : IRequest<UserLoginResponse>;

public record UserLoginResponse(int Id, string Name, string Surname, string Email, string GroupName);
