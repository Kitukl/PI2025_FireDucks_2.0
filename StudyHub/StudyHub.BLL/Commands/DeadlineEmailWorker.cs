
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudyHub.BLL.Commands;
using StudyHub.DAL.Repositories;
using Task = System.Threading.Tasks.Task;

namespace StudyHub.BLL.Workers;

public class DeadlineEmailWorker : BackgroundService
{
    private readonly ILogger<DeadlineEmailWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private readonly HashSet<int> _notifiedTaskIds = new();

    public DeadlineEmailWorker(
        ILogger<DeadlineEmailWorker> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        _logger.LogInformation("DeadlineEmailWorker constructed");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("DeadlineEmailWorker started (interval: 4s)");

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(4));
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
            using var scope = _scopeFactory.CreateScope();

            var taskRepo = scope.ServiceProvider
                .GetRequiredService<IBaseRepository<StudyHub.DAL.Entities.Task>>();

            var emailSender = scope.ServiceProvider
                .GetRequiredService<EmailSender>();

            var now = DateTime.Now;

            var tasks = await taskRepo.GetAll();
            _logger.LogInformation("RunOnce: loaded {Count} tasks", tasks.Count);

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

                var info = $"{task.Title} — {(task.Deadline - task.CreationDate).TotalDays} днів до дедлайну";

                _logger.LogInformation("Sending email for Task {Id} to {Email}", task.Id, task.User.Email);

                await emailSender.Send(
                    to: task.User.Email,
                    subject: "Ваше завдання близько до дедлайну!",
                    messageText: info);

                _notifiedTaskIds.Add(task.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeadlineEmailWorker failed");
        }
    }
}