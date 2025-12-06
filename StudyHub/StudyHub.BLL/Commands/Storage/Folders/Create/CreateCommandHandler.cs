using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Commands.Storage.Folder.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand>
{
    private readonly IFileStorageService _fileStorage;

    public CreateCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async System.Threading.Tasks.Task Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        await _fileStorage.CreateFolderAsync(request.UserId, request.FolderPath);
    }
}