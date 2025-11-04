using System.Threading;
using System.Threading.Tasks;
using MediatR;
using BCrypt.Net;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.User.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, int>
    {
        private readonly IBaseRepository<DAL.Entities.User> _userRepository;

        public RegisterCommandHandler(IBaseRepository<DAL.Entities.User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> Handle(RegisterCommand request, CancellationToken cancellationToken){
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.password);

            var user = new DAL.Entities.User
            {
                Name = request.name,
                Surname = request.surname,
                Email = request.email,
                Password = hashedPassword,
                UserPhoto = null,
                DaysForNotification = 7,
                IsNotified = false,
                Schedule = new List<Schedule>(),
                Tasks = new List<DAL.Entities.Task>(),
                Tickets = new List<SupportTicket>()
            };

            var created = await _userRepository.CreateAsync(user);

            return created.Id;
        }
    }
}