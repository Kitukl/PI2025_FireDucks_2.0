using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.User.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, UserLoginResponse>
{
    private readonly UserRepository _userRepository;

    public LoginCommandHandler(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserLoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmail(request.email);
        if (user == null)
            throw new InvalidOperationException("Користувача з таким email не знайдено");

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.password, user.Password);
        if (!isPasswordValid)
            throw new InvalidOperationException("Невірний пароль");

        return new UserLoginResponse(
            user.Id,
            user.Name,
            user.Surname,
            user.Email
        );
    }
}