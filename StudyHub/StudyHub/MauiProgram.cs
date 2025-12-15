
using dotenv.net;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SendGrid;
using Serilog;
using StudyHub.BLL.Commands;
using StudyHub.BLL.Commands.User.Register;
using StudyHub.BLL.Interfaces;
using StudyHub.BLL.Services;
using StudyHub.BLL.Workers;
using StudyHub.DAL;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;

namespace StudyHub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            DotEnv.Load();

            var logDirectory = Path.Combine(FileSystem.AppDataDirectory, "logs");
            Directory.CreateDirectory(logDirectory);
            var logPath = Path.Combine(logDirectory, "studyhub-.txt");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30
                )
                .CreateLogger();

            try
            {
                Log.Information("StudyHub застосунок запускається...");
                Log.Information("Шлях до логів: {LogPath}", logDirectory);

                var builder = MauiApp.CreateBuilder();

                builder
                    .UseMauiApp<App>()
                    .ConfigureFonts(fonts =>
                    {
                        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    });

                builder.Services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.AddSerilog(dispose: true);
                });

                builder.Services.AddMauiBlazorWebView();

                AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

                // Репозиторії (scoped)
                builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();
                builder.Services.AddScoped<IBaseRepository<Lecturer>, LecturerRepository>();
                builder.Services.AddScoped<IBaseRepository<Lesson>, LessonRepository>();
                builder.Services.AddScoped<IBaseRepository<Subject>, SubjectRepository>();
                builder.Services.AddScoped<IBaseRepository<Schedule>, ScheduleRepository>();
                builder.Services.AddScoped<IBaseRepository<LessonSlots>, LessonSlotsRepository>();
                builder.Services.AddScoped<IBaseRepository<Comments>, CommentsRepository>();
                builder.Services.AddScoped<IBaseRepository<StudyHub.DAL.Entities.Task>, TaskRepository>();

                // Сервіси
                builder.Services.AddScoped<StudyContext>();
                builder.Services.AddScoped<UserRepository>();
                builder.Services.AddScoped<TaskRepository>();
                builder.Services.AddScoped<CommentsRepository>();
                builder.Services.AddScoped<SupportTicketRepository>();
                builder.Services.AddScoped<CategoryRepository>();

                builder.Services.AddScoped<IParserRunner, ParserRunner>();
                builder.Services.AddScoped<IFacultyHelper, AmiFacultyHelper>();
                builder.Services.AddScoped<ScheduleService>();
                builder.Services.AddScoped<UniversityService>();
                builder.Services.AddScoped<IScheduleDownloaderService, ScheduleDownloaderService>();
                builder.Services.AddSingleton<AuthStateService>();

                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<ITaskService, TaskService>();
                builder.Services.AddScoped<EmailSender>();

                // SendGrid
                builder.Services.AddScoped<ISendGridClient>(_ =>
                    new SendGridClient(""));

                // Локальне сховище
                builder.Services.AddSingleton<IFileStorageService>(_ =>
                    new LocalFileStorageService(Path.Combine(Directory.GetCurrentDirectory(), "Storage")));

                builder.Services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));

                // Hosted Service
                builder.Services.AddHostedService<DeadlineEmailWorker>();

#if DEBUG
                builder.Services.AddBlazorWebViewDeveloperTools();
#endif

                Log.Information("Сервіси успішно зареєстровано");

                var app = builder.Build();

                // Явний старт Hosted Services
                System.Threading.Tasks.Task.Run(async () =>
                {
                    var hostedServices = app.Services.GetServices<IHostedService>();
                    foreach (var hosted in hostedServices)
                    {
                        await hosted.StartAsync(CancellationToken.None);
                    }
                });

                Log.Information("StudyHub застосунок успішно запущено");

                return app;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Критична помилка при запуску застосунку");
                throw;
            }
        }
    }
}