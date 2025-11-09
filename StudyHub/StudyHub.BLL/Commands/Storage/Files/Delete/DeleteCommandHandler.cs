using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Commands.Storage.Files.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand>
{
    private readonly IFileStorageService _fileStorage;

    public DeleteCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async System.Threading.Tasks.Task Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        await _fileStorage.DeleteFileAsync(request.FilePath);
    }
}