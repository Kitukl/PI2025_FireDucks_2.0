using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Commands.Storage.Files.Delete;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteCommand>
{
    private readonly IFileStorageService _fileStorage;

    public DeleteTaskCommandHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async System.Threading.Tasks.Task Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        await _fileStorage.DeleteFileAsync(request.UserId, request.FilePath);
    }
}