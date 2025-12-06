using MediatR;
using StudyHub.DAL.Repositories;
using StudyHub.BLL.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DAL_Task = StudyHub.DAL.Entities.Task;
using Status = StudyHub.DAL.Entities.Status;

namespace StudyHub.BLL.Queries.Task
{
    public class GetUserTasksQueryHandler : IRequestHandler<GetUserTasksQuery, List<TaskItem>>
    {
        private readonly IBaseRepository<DAL_Task> _taskRepository;

        public GetUserTasksQueryHandler(IBaseRepository<DAL_Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async System.Threading.Tasks.Task<List<TaskItem>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
        {
            var tasks = await _taskRepository.GetAll();

            var userTasks = tasks
                .Where(t => t.UserId == request.UserId)
                .ToList();

            return userTasks.Select(t => new TaskItem
            {
                Id = t.Id,
                Name = t.Title ?? "No Title",
                Description = t.Description ?? "",
                Subject = t.User?.GroupName ?? "No Subject",
                Code = GenerateTaskCode(t),
                Status = MapStatusToString(t.Status),
                DueDate = t.Deadline,
                CreationDate = t.CreationDate,
                ReminderDays = t.User?.DaysForNotification ?? 2
            }).ToList();
        }

        private string GenerateTaskCode(DAL_Task task)
        {
            if (task == null) return "T-0";
            var subject = task.User?.GroupName ?? "T";
            var prefix = string.IsNullOrEmpty(subject) ? "T" : subject.Substring(0, 1).ToUpper();
            return $"{prefix}-{task.Id}";
        }

        private string MapStatusToString(Status status)
        {
            return status switch
            {
                Status.InProgress => "In Progress",
                Status.ReadyForReview => "For Review",
                Status.Done => "Done",
                _ => "To Do"
            };
        }
    }
}