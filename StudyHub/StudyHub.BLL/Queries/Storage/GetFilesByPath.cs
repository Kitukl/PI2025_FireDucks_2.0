using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Queries.Storage;

public record GetFilesByPath(int UserId, string Path) : IRequest<IEnumerable<string>>;

public class GetFilesByPathHandler : IRequestHandler<GetFilesByPath, IEnumerable<string>>
{
    private readonly IFileStorageService _fileStorage;

    public GetFilesByPathHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<IEnumerable<string>> Handle(GetFilesByPath request, CancellationToken cancellationToken)
    {
        var files = await _fileStorage.GetFilesAsync(request.UserId, request.Path);
        return files.OrderBy(f => f);
    }
}