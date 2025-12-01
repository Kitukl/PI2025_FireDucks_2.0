using Microsoft.Extensions.Logging;
using StudyHub.BLL.Services;
using StudyHub.DAL.Entities;
using StudyHub.DAL.Repositories;
using StudyHub.DAL;

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

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddScoped<StudyContext>();

            builder.Services.AddScoped<IBaseRepository<Lecturer>, LecturerRepository>();
            builder.Services.AddScoped<IBaseRepository<Lesson>, LessonRepository>();
            builder.Services.AddScoped<IBaseRepository<Subject>, SubjectRepository>();
            builder.Services.AddScoped<IBaseRepository<Schedule>, ScheduleRepository>();

            builder.Services.AddScoped<IBaseRepository<LessonSlots>, LessonSlotsRepository>();

            builder.Services.AddScoped<ScheduleService>();

            builder.Services.AddScoped<IFacultyHelper, AMIFacultyHelper>();

            builder.Services.AddScoped<UniversityService>();

            builder.Services.AddScoped<IScheduleDownloaderService, ScheduleDownloaderService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}