using Castle.Core.Smtp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudyHub.BLL.Commands;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.BLL.Workers;

public class DeadlineEmailWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<DeadlineEmailWorker> _logger;

    private readonly HashSet<int> _notifiedTaskIds = new();

    public DeadlineEmailWorker(
        IServiceProvider services,
        ILogger<DeadlineEmailWorker> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));
        await RunOnce(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunOnce(stoppingToken);
        }
    }

    private async Task RunOnce(CancellationToken ct)
    {
        try
        {
            using var scope = _services.CreateScope();

            var taskRepo = scope.ServiceProvider
                .GetRequiredService<IBaseRepository<StudyHub.DAL.Entities.Task>>();

            var emailService = scope.ServiceProvider
                .GetRequiredService<EmailSender>();

            var now = DateTime.Now;

            var tasks = await taskRepo.GetAll();

            foreach (var task in tasks)
            {
                if (_notifiedTaskIds.Contains(task.Id))
                    continue;

                if (task.User == null ||
                    string.IsNullOrWhiteSpace(task.User.Email) || !task.User.IsNotified)
                    continue;

                if (task.Deadline <= now)
                    continue;

                var notifyThreshold = now.AddDays(task.User.DaysForNotification);
                if (notifyThreshold < task.Deadline)
                    continue;

                var info = $"{task.Title} — {task.User.DaysForNotification} днів до дедлайну";

                await emailService.Send(
                    to: task.User.Email,
                    subject: "Ваше завдання близько до дедлайну!",
                    info);
                _notifiedTaskIds.Add(task.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeadlineEmailWorker failed");
        }
    }
}
