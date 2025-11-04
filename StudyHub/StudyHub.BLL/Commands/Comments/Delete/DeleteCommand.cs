using MediatR;

namespace StudyHub.BLL.Commands.Comments.Delete;

public record DeleteCommand(int id): IRequest<int>;