using Microsoft.Extensions.Logging;
using StudyHub.BLL.Interfaces;
using StudyHub.BLL.Services;
using StudyHub.DAL;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using MediatR;
using StudyHub.BLL.Commands.User.Register;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace StudyHub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped<StudyContext>();

            builder.Services.AddScoped<UserRepository>();
            builder.Services.AddScoped<TaskRepository>();
            builder.Services.AddScoped<CommentsRepository>();

            builder.Services.AddScoped<IBaseRepository<User>, UserRepository>();
            builder.Services.AddScoped<IBaseRepository<Lecturer>, LecturerRepository>();
            builder.Services.AddScoped<IBaseRepository<Lesson>, LessonRepository>();
            builder.Services.AddScoped<IBaseRepository<Subject>, SubjectRepository>();
            builder.Services.AddScoped<IBaseRepository<Schedule>, ScheduleRepository>();
            builder.Services.AddScoped<IBaseRepository<LessonSlots>, LessonSlotsRepository>();
            builder.Services.AddScoped<IBaseRepository<Comments>, CommentsRepository>();

            builder.Services.AddScoped<IBaseRepository<DAL.Entities.Task>, TaskRepository>();

            builder.Services.AddScoped<IParserRunner, ParserRunner>();
            builder.Services.AddScoped<IFacultyHelper, AmiFacultyHelper>();

            builder.Services.AddScoped<ScheduleService>();
            builder.Services.AddScoped<UniversityService>();
            builder.Services.AddScoped<IScheduleDownloaderService, ScheduleDownloaderService>();
            builder.Services.AddSingleton<AuthStateService>();

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterCommandHandler).Assembly));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddSingleton<IFileStorageService>(sp =>
    new LocalFileStorageService(Path.Combine(Directory.GetCurrentDirectory(), "Storage")));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}