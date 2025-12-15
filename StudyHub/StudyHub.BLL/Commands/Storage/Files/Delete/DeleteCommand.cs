using MediatR;

namespace StudyHub.BLL.Commands.Storage.Files.Delete;

public record DeleteCommand(int UserId, string FilePath): IRequest;