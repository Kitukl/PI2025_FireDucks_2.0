using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Commands.Storage.Files.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand>
{
    private readonly IFileStorageService _fileStorage;

    public CreateCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async System.Threading.Tasks.Task Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        await _fileStorage.AddFileAsync(request.FolderPath, request.FileStream, request.FileName);
    }
}