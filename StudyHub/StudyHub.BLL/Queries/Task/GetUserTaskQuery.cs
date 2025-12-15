using MediatR;
using StudyHub.BLL.Models; 
using System.Collections.Generic; 

namespace StudyHub.BLL.Queries.Task;
public record GetUserTasksQuery(int UserId) : IRequest<List<TaskItem>>;