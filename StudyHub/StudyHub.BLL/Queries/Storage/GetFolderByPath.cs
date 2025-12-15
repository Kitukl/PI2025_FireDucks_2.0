using MediatR;
using StudyHub.BLL.Services;

namespace StudyHub.BLL.Queries.Storage;

public record GetFoldersQuery(int UserId, string Path) : IRequest<IEnumerable<string>>;

public class GetFoldersHandler : IRequestHandler<GetFoldersQuery, IEnumerable<string>>
{
    private readonly IFileStorageService _fileStorage;

    public GetFoldersHandler(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    public async Task<IEnumerable<string>> Handle(GetFoldersQuery request, CancellationToken cancellationToken)
    {
        return await _fileStorage.GetFoldersAsync(request.UserId, request.Path);
    }
}