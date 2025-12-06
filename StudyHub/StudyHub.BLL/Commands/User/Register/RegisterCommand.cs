using MediatR;

namespace StudyHub.BLL.Commands.User.Register;

public record RegisterCommand(string name, string surname, string email, string password, string groupName) : IRequest<int>;