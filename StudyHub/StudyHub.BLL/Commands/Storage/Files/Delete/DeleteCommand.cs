using MediatR;

namespace StudyHub.BLL.Commands.Storage.Files.Delete;

public record DeleteCommand(string FilePath): IRequest;