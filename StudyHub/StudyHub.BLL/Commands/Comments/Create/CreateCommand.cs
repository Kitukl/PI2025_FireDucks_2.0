using MediatR;

namespace StudyHub.BLL.Commands.Comments.Create;

public record CreateCommand(string description, int taskId): IRequest<int>;