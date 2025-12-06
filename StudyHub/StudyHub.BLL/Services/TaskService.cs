using MediatR;
using StudyHub.BLL.Commands.Comments.Create;
using StudyHub.BLL.Commands.Comments.Delete;
using StudyHub.BLL.Commands.Comments.Update;
using StudyHub.BLL.Commands.Task.Create;
using StudyHub.BLL.Commands.Task.Delete;
using StudyHub.BLL.Commands.Task.Update;
using StudyHub.BLL.Models;
using StudyHub.BLL.Queries.CommentQueries;
using StudyHub.BLL.Queries.Task;
using StudyHub.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskEntity = StudyHub.DAL.Entities.Task;
using CreateTaskCommand = StudyHub.BLL.Commands.Task.Create.CreateCommand;
using CreateCommentCommand = StudyHub.BLL.Commands.Comments.Create.CreateCommand;
using DeleteTaskCommand = StudyHub.BLL.Commands.Task.Delete.DeleteCommand;
using DeleteCommentCommand = StudyHub.BLL.Commands.Comments.Delete.DeleteCommand;
using UpdateCommentCommand = StudyHub.BLL.Commands.Comments.Update.UpdateCommand;

namespace StudyHub.BLL.Services
{
    public interface ITaskService
    {
        System.Threading.Tasks.Task<List<TaskItem>> GetUserTasksAsync(int userId);
        System.Threading.Tasks.Task<int> CreateTaskAsync(int userId, string title, string description, string subject, DateTime? deadline, Status status);
        System.Threading.Tasks.Task<bool> UpdateTaskAsync(int taskId, string title, string description, string subject, string status, DateTime? deadline);
        System.Threading.Tasks.Task<bool> DeleteTaskAsync(int taskId);

        System.Threading.Tasks.Task<int> CreateCommentAsync(int taskId, int userId, string description);
        System.Threading.Tasks.Task<List<CommentItem>> GetTaskCommentsAsync(int taskId);
        System.Threading.Tasks.Task<bool> UpdateCommentAsync(int commentId, string description);
        System.Threading.Tasks.Task<bool> DeleteCommentAsync(int commentId);
    }

    public class TaskService : ITaskService
    {
        private readonly IMediator _mediator;

        public TaskService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async System.Threading.Tasks.Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            var query = new GetUserTasksQuery(userId);
            return await _mediator.Send(query);
        }

        public async System.Threading.Tasks.Task<int> CreateTaskAsync(int userId, string title, string description, string subject, DateTime? deadline, Status status)
        {
            var command = new CreateTaskCommand(
                userId,
                title,
                description,
                deadline ?? DateTime.Now.AddDays(7),
                status
            );
            return await _mediator.Send(command);
        }

        public async System.Threading.Tasks.Task<bool> UpdateTaskAsync(int taskId, string title, string description, string subject, string status, DateTime? deadline)
        {
            var taskEntity = new TaskEntity
            {
                Id = taskId,
                Title = title,
                Description = description,
                Deadline = deadline ?? DateTime.Now.AddDays(7),
                Status = MapStringToStatus(status)
            };

            var command = new StudyHub.BLL.Commands.Task.Update.UpdateCommand(taskEntity);
            var resultId = await _mediator.Send(command);
            return resultId > 0;
        }

        public async System.Threading.Tasks.Task<bool> DeleteTaskAsync(int taskId)
        {
            var command = new DeleteTaskCommand(taskId);
            var resultId = await _mediator.Send(command);
            return resultId > 0;
        }

        public async System.Threading.Tasks.Task<int> CreateCommentAsync(int taskId, int userId, string description)
        {
            var command = new CreateCommentCommand(description, taskId, userId);
            return await _mediator.Send(command);
        }

        public async System.Threading.Tasks.Task<List<CommentItem>> GetTaskCommentsAsync(int taskId)
        {
            var query = new GetTaskCommentsQuery(taskId);
            var comments = await _mediator.Send(query);

            return comments.Select(c => new CommentItem
            {
                Id = c.Id,
                Description = c.Description,
                CreationDate = c.CreationDate,
                TaskId = c.TaskId,
                UserId = c.UserId,
                AuthorName = c.User != null ? $"{c.User.Name} {c.User.Surname}" : "Unknown User"
            }).ToList();
        }

        public async System.Threading.Tasks.Task<bool> UpdateCommentAsync(int commentId, string description)
        {
            var comment = new Comments
            {
                Id = commentId,
                Description = description
            };

            var command = new UpdateCommentCommand(comment);
            var resultId = await _mediator.Send(command);
            return resultId > 0;
        }

        public async System.Threading.Tasks.Task<bool> DeleteCommentAsync(int commentId)
        {
            var command = new DeleteCommentCommand(commentId);
            var resultId = await _mediator.Send(command);
            return resultId > 0;
        }

        private Status MapStringToStatus(string status)
        {
            return status.ToLower() switch
            {
                "to do" => Status.ToDo,
                "in progress" => Status.InProgress,
                "for review" => Status.ReadyForReview,
                "done" => Status.Done,
                _ => Status.InProgress
            };
        }
    }
}