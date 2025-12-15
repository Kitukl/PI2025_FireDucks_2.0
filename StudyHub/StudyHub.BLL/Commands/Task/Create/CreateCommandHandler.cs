using MediatR;
using StudyHub.DAL.Repositories;
using System;

namespace StudyHub.BLL.Commands.Task.Create
{
    public class CreateCommandHandler : IRequestHandler<CreateCommand, int>
    {
        private readonly IBaseRepository<DAL.Entities.Task> _taskRepository;

        public CreateCommandHandler(IBaseRepository<DAL.Entities.Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<int> Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            try
            {
                Console.WriteLine($"Creating task: UserId={request.UserId}, Title={request.Title}");

                var task = new DAL.Entities.Task
                {
                    Title = request.Title,
                    Description = request.Description,
                    Deadline = request.Deadline.ToUniversalTime(),
                    Status = request.Status,
                    CreationDate = DateTime.UtcNow,
                    UserId = request.UserId
                };

                Console.WriteLine($"Task created in memory: UserId={task.UserId}");

                var created = await _taskRepository.CreateAsync(task);

                Console.WriteLine($"Task saved to DB: Id={created.Id}");

                return created.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating task: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}