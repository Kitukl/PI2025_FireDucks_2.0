using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Task.Delete;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteCommand, int>
{
    private readonly TaskRepository _taskRepository;

    public DeleteTaskCommandHandler(TaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    public async Task<int> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        return await _taskRepository.DeleteAsync(request.id);
    }
}