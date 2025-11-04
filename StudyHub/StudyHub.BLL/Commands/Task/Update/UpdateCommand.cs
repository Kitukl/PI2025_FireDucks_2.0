using MediatR;

namespace StudyHub.BLL.Commands.Task.Update;

public record UpdateCommand(DAL.Entities.Task task) : IRequest<int>;