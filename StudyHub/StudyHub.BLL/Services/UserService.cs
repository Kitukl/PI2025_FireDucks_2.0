using MediatR;
using StudyHub.BLL.Commands.User.Register;
using StudyHub.BLL.Commands.User.Login;

namespace StudyHub.BLL.Services
{
    public interface IUserService
    {
        Task<int> RegisterUserAsync(string name, string surname, string email, string password, string groupName);
        Task<UserLoginResponse> LoginAsync(string email, string password);
    }

    public class UserService : IUserService
    {
        private readonly IMediator _mediator;

        public UserService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<int> RegisterUserAsync(string name, string surname, string email, string password, string groupName)
        {
            var command = new RegisterCommand(name, surname, email, password, groupName);
            return await _mediator.Send(command);
        }

        public async Task<UserLoginResponse> LoginAsync(string email, string password)
        {
            var command = new LoginCommand(email, password);
            return await _mediator.Send(command);
        }
    }
}