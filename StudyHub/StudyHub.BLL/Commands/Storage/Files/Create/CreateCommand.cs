using MediatR;

namespace StudyHub.BLL.Commands.Storage.Files.Create;

public record CreateCommand(int UserId, string FolderPath, byte[] FileData, string FileName) : IRequest;