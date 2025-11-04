using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Commands.Task.Update;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, int>
{
    private readonly TaskRepository _taskRepository;

    public UpdateCommandHandler(TaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }
    public async Task<int> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
       return await _taskRepository.UpdateAsync(request.task);
    }
}