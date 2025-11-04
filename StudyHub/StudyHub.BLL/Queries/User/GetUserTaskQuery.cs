using MediatR;
using StudyHub.DAL.Repositories;

namespace StudyHub.BLL.Queries.TaskQueries;

public record GetUserTasksQuery(int UserId) : IRequest<List<StudyHub.DAL.Entities.Task>>;

public class GetUserTasksQueryHandler : IRequestHandler<GetUserTasksQuery, List<StudyHub.DAL.Entities.Task>>
{
    private readonly TaskRepository _taskRepository;

    public GetUserTasksQueryHandler(TaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<List<StudyHub.DAL.Entities.Task>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByUser(request.UserId);
        return tasks;
    }
}