using MediatR;

namespace StudyHub.BLL.Commands.Storage.Folder.Create;

public record CreateCommand(string FolderPath) : IRequest;
