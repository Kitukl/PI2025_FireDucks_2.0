using MediatR;

namespace StudyHub.BLL.Commands.Task.Delete;

public record DeleteCommand(int id) : IRequest<int>;