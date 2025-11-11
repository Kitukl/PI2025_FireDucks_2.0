using MediatR;

namespace StudyHub.BLL.Commands.Storage.Files.Create;

public record CreateCommand(string FolderPath, Stream FileStream, string FileName) : IRequest;
