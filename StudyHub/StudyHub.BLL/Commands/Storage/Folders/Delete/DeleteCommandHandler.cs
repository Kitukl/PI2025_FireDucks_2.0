using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Storage.Folder.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, int>
{
    private readonly CommentsRepository _repository;

    public DeleteCommandHandler(CommentsRepository repository)
    {
        _repository = repository;
    }
    public async Task<int> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.id);
    }
}