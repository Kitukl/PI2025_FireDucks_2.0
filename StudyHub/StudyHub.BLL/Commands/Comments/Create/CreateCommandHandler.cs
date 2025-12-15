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
        Console.WriteLine($"=== CREATE COMMENT HANDLER ===");
        Console.WriteLine($"TaskId: {request.taskId}");
        Console.WriteLine($"UserId: {request.userId}");
        Console.WriteLine($"Description: {request.description}");

        var task = await _taskRepository.GetById(request.taskId);
        if (task is null)
        {
            throw new InvalidOperationException($"Task with Id {request.taskId} not found");
        }

        var comment = new DAL.Entities.Comments
        {
            Description = request.description,
            TaskId = request.taskId,
            UserId = request.userId,
            CreationDate = DateTime.UtcNow
        };

        Console.WriteLine($"Calling repository.CreateAsync...");

        await _repository.CreateAsync(comment);

        Console.WriteLine($"Comment saved with Id: {comment.Id}");

        return comment.Id;
    }
}