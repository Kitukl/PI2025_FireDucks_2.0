using MediatR;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<TaskService> _logger;

        // ✅ ВАЖЛИВО: Конструктор з ILogger
        public TaskService(IMediator mediator, ILogger<TaskService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            _logger.LogInformation("Отримання завдань користувача: UserId={UserId}", userId);

            try
            {
                var query = new GetUserTasksQuery(userId);
                var tasks = await _mediator.Send(query);

                _logger.LogInformation("Отримано {TaskCount} завдань для користувача: UserId={UserId}",
                    tasks.Count, userId);

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні завдань користувача: UserId={UserId}", userId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<int> CreateTaskAsync(int userId, string title, string description, string subject, DateTime? deadline, Status status)
        {
            _logger.LogInformation("Створення завдання: UserId={UserId}, Title={Title}, Status={Status}, Deadline={Deadline}",
                userId, title, status, deadline);

            try
            {
                var command = new CreateTaskCommand(
                    userId,
                    title,
                    description,
                    deadline ?? DateTime.Now.AddDays(7),
                    status
                );

                var taskId = await _mediator.Send(command);

                _logger.LogInformation("Завдання успішно створено: TaskId={TaskId}, UserId={UserId}",
                    taskId, userId);

                return taskId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні завдання: UserId={UserId}, Title={Title}",
                    userId, title);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> UpdateTaskAsync(int taskId, string title, string description, string subject, string status, DateTime? deadline)
        {
            _logger.LogInformation("Оновлення завдання: TaskId={TaskId}, Title={Title}, Status={Status}",
                taskId, title, status);

            try
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

                if (resultId > 0)
                {
                    _logger.LogInformation("Завдання успішно оновлено: TaskId={TaskId}", taskId);
                }
                else
                {
                    _logger.LogWarning("Не вдалося оновити завдання: TaskId={TaskId}", taskId);
                }

                return resultId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні завдання: TaskId={TaskId}", taskId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> DeleteTaskAsync(int taskId)
        {
            _logger.LogInformation("Видалення завдання: TaskId={TaskId}", taskId);

            try
            {
                var command = new DeleteTaskCommand(taskId);
                var resultId = await _mediator.Send(command);

                if (resultId > 0)
                {
                    _logger.LogInformation("Завдання успішно видалено: TaskId={TaskId}", taskId);
                }
                else
                {
                    _logger.LogWarning("Не вдалося видалити завдання: TaskId={TaskId}", taskId);
                }

                return resultId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні завдання: TaskId={TaskId}", taskId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<int> CreateCommentAsync(int taskId, int userId, string description)
        {
            _logger.LogInformation("Створення коментаря: TaskId={TaskId}, UserId={UserId}",
                taskId, userId);

            try
            {
                var command = new CreateCommentCommand(description, taskId, userId);
                var commentId = await _mediator.Send(command);

                _logger.LogInformation("Коментар успішно створено: CommentId={CommentId}, TaskId={TaskId}",
                    commentId, taskId);

                return commentId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні коментаря: TaskId={TaskId}, UserId={UserId}",
                    taskId, userId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<List<CommentItem>> GetTaskCommentsAsync(int taskId)
        {
            _logger.LogInformation("Отримання коментарів завдання: TaskId={TaskId}", taskId);

            try
            {
                var query = new GetTaskCommentsQuery(taskId);
                var comments = await _mediator.Send(query);

                var commentItems = comments.Select(c => new CommentItem
                {
                    Id = c.Id,
                    Description = c.Description,
                    CreationDate = c.CreationDate,
                    TaskId = c.TaskId,
                    UserId = c.UserId,
                    AuthorName = c.User != null ? $"{c.User.Name} {c.User.Surname}" : "Unknown User"
                }).ToList();

                _logger.LogInformation("Отримано {CommentCount} коментарів для завдання: TaskId={TaskId}",
                    commentItems.Count, taskId);

                return commentItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні коментарів: TaskId={TaskId}", taskId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> UpdateCommentAsync(int commentId, string description)
        {
            _logger.LogInformation("Оновлення коментаря: CommentId={CommentId}", commentId);

            try
            {
                var comment = new Comments
                {
                    Id = commentId,
                    Description = description
                };

                var command = new UpdateCommentCommand(comment);
                var resultId = await _mediator.Send(command);

                if (resultId > 0)
                {
                    _logger.LogInformation("Коментар успішно оновлено: CommentId={CommentId}", commentId);
                }
                else
                {
                    _logger.LogWarning("Не вдалося оновити коментар: CommentId={CommentId}", commentId);
                }

                return resultId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні коментаря: CommentId={CommentId}", commentId);
                throw;
            }
        }

        public async System.Threading.Tasks.Task<bool> DeleteCommentAsync(int commentId)
        {
            _logger.LogInformation("Видалення коментаря: CommentId={CommentId}", commentId);

            try
            {
                var command = new DeleteCommentCommand(commentId);
                var resultId = await _mediator.Send(command);

                if (resultId > 0)
                {
                    _logger.LogInformation("Коментар успішно видалено: CommentId={CommentId}", commentId);
                }
                else
                {
                    _logger.LogWarning("Не вдалося видалити коментар: CommentId={CommentId}", commentId);
                }

                return resultId > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні коментаря: CommentId={CommentId}", commentId);
                throw;
            }
        }

        private Status MapStringToStatus(string status)
        {
            var mappedStatus = status.ToLower() switch
            {
                "to do" => Status.ToDo,
                "in progress" => Status.InProgress,
                "for review" => Status.ReadyForReview,
                "done" => Status.Done,
                _ => Status.InProgress
            };

            _logger.LogDebug("Маппінг статусу: '{StatusString}' -> {MappedStatus}", status, mappedStatus);

            return mappedStatus;
        }
    }
}