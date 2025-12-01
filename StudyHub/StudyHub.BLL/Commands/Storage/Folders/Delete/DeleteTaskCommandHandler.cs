using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Storage.Folder.Delete;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteCommand, int>
{
    private readonly CommentsRepository _repository;

    public DeleteTaskCommandHandler(CommentsRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.id);
    }
}