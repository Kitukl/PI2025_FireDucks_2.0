using MediatR;

namespace StudyHub.BLL.Commands.Storage.Folder.Delete;

public record DeleteCommand(int id): IRequest<int>;