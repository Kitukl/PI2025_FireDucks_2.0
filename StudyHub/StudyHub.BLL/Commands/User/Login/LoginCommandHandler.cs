using MediatR;
using StudyHub.DAL.Repositories;
using StudyHub.DAL.Entities;

namespace StudyHub.BLL.Commands.User.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, UserLoginResponse>
    {
        private readonly UserRepository _userRepository;

        public LoginCommandHandler(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserLoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByEmail(request.email);

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.password, user.Password);

                if (!isPasswordValid)
                    throw new InvalidOperationException("Неправильний пароль");

                return new UserLoginResponse(
                    user.Id,
                    user.Name,
                    user.Surname,
                    user.Email,
                    user.GroupName
                );
            }
            catch (Exception ex) when (ex.Message.Contains("Not found user"))
            {
                throw new InvalidOperationException("Користувача з таким email не знайдено");
            }
        }
    }
}