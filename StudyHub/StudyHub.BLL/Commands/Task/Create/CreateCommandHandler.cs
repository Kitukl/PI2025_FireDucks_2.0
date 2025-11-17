using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Task.Create;

public class CreateCommandHandler : IRequestHandler<CreateCommand, int>
{
    private readonly TaskRepository _taskRepository;
    private readonly UserRepository _userRepository;

    public CreateCommandHandler(TaskRepository taskRepository, UserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<int> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.userId);
        if (user == null) throw new InvalidOperationException("Користувача не знайдено");
        var task = new DAL.Entities.Task()
        {
            CreationDate = DateTime.Now,
            Deadline = request.deadline,
            Description = request.description,
            Status = request.status,
            Title = request.title,
            User = user
        };
        await _taskRepository.CreateAsync(task);

        return task.Id;
    }
}