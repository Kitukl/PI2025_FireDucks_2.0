using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Comments.Update;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, int>
{
    private readonly CommentsRepository _repository;

    public UpdateCommandHandler(CommentsRepository repository)
    {
        _repository = repository;
    }
    public async Task<int> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        return await _repository.UpdateAsync(request.comment);
    }
}