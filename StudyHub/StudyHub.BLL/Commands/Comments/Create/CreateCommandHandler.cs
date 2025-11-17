using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Comments.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand, int>
{
    private readonly CommentsRepository _repository;
    private readonly TaskRepository _taskRepository;

    public CreateCommandHandler(CommentsRepository repository, TaskRepository taskRepository)
    {
        _repository = repository;
        _taskRepository = taskRepository;
    }

    public async Task<int> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(request.taskId);
        var comment = new DAL.Entities.Comments
        {
            Description = request.description,
            Task = task,
            CreationDate = DateTime.UtcNow,
        };
        await _repository.CreateAsync(comment);

        return comment.Id;
    }
}