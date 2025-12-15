using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Commands.Storage.Folder.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, int>
{
    private readonly IFileStorageService _fileStorage;

    public DeleteCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<int> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        await _fileStorage.DeleteFolderAsync(request.UserId, request.FolderPath);
        return 1;
    }
}