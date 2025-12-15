using MediatR;

namespace StudyHub.BLL.Commands.Storage.Folder.Create;

public record CreateCommand(int UserId, string FolderPath) : IRequest;
