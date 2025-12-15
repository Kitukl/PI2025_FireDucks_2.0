using MediatR;
using Microsoft.Extensions.Logging;
using StudyHub.BLL.Commands.User.Register;
using StudyHub.BLL.Commands.User.Login;

namespace StudyHub.BLL.Services
{
    public interface IUserService
    {
        Task<int> RegisterUserAsync(string name, string surname, string email, string password, string groupName);
        Task<UserLoginResponse> LoginAsync(string email, string password);
        Task<int> SendContactFormAsync(string type, string category, string subject, string description, int userId);
    }

    public class UserService : IUserService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserService> _logger;

        public UserService(IMediator mediator, ILogger<UserService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<int> RegisterUserAsync(string name, string surname, string email, string password, string groupName)
        {
            _logger.LogInformation("Початок реєстрації користувача: Email={Email}, Group={Group}", email, groupName);

            try
            {
                var command = new RegisterCommand(name, surname, email, password, groupName);
                var userId = await _mediator.Send(command);

                _logger.LogInformation("Користувача успішно зареєстровано: UserId={UserId}, Email={Email}, Group={Group}",
                    userId, email, groupName);

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при реєстрації користувача: Email={Email}, Group={Group}",
                    email, groupName);
                throw;
            }
        }

        public async Task<UserLoginResponse> LoginAsync(string email, string password)
        {
            _logger.LogInformation("Спроба входу користувача: Email={Email}", email);

            try
            {
                var command = new LoginCommand(email, password);
                var response = await _mediator.Send(command);

                if (response != null && response.Id > 0)
                {
                    _logger.LogInformation("Користувач успішно увійшов: UserId={UserId}, Email={Email}",
                        response.Id, email);
                }
                else
                {
                    _logger.LogWarning("Невдала спроба входу: Email={Email}", email);
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при вході користувача: Email={Email}", email);
                throw;
            }
        }

        public async Task<int> SendContactFormAsync(string type, string category, string subject, string description, int userId)
        {
            _logger.LogInformation("Створення звернення до підтримки: UserId={UserId}, Type={Type}, Category={Category}, Subject={Subject}",
                userId, type, category, subject);

            try
            {
                var command = new Commands.SupportTickets.Create.CreateCommand(
                    userId,
                    type,
                    category,
                    subject,
                    description
                );

                var ticketId = await _mediator.Send(command);

                _logger.LogInformation("Звернення успішно створено: TicketId={TicketId}, UserId={UserId}",
                    ticketId, userId);

                return ticketId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні звернення: UserId={UserId}, Type={Type}",
                    userId, type);
                throw;
            }
        }
    }
}