using Castle.Core.Smtp;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StudyHub.BLL.Interfaces;
using SendGrid;
using StudyHub.BLL.Commands;
using StudyHub.BLL.Services;
using StudyHub.BLL.Workers;
using StudyHub.DAL;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using Task = StudyHub.DAL.Entities.Task;

namespace StudyHub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            DotEnv.Load();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped<StudyContext>();

            builder.Services.AddScoped<IBaseRepository<Lecturer>, LecturerRepository>();
            builder.Services.AddScoped<IBaseRepository<Lesson>, LessonRepository>();
            builder.Services.AddScoped<IBaseRepository<Subject>, SubjectRepository>();
            builder.Services.AddScoped<IBaseRepository<Schedule>, ScheduleRepository>();
            builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();
            builder.Services.AddScoped<IBaseRepository<Task>, TaskRepository>();

            builder.Services.AddScoped<IBaseRepository<LessonSlots>, LessonSlotsRepository>();
            
            builder.Services.AddHostedService<DeadlineEmailWorker>();

            builder.Services.AddScoped<IParserRunner, ParserRunner>();

            builder.Services.AddScoped<ScheduleService>();

            builder.Services.AddScoped<IFacultyHelper, AmiFacultyHelper>();

            builder.Services.AddScoped<UniversityService>();

            builder.Services.AddScoped<IScheduleDownloaderService, ScheduleDownloaderService>();
            builder.Services.AddSingleton<ScheduleService>();

            builder.Services.AddScoped<EmailSender>();
            
            builder.Services.AddScoped<ISendGridClient>(sg => new SendGridClient(Environment.GetEnvironmentVariable("API_KEY")));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}