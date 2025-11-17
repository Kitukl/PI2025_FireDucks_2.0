using Microsoft.Extensions.Logging;
using StudyHub.BLL.Queries.Storage;
using StudyHub.BLL.Queries.TaskQueries;
using StudyHub.BLL.Services;

namespace StudyHub;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetUserTasksQueryHandler).Assembly);
        });

        builder.Services.AddSingleton<IFileStorageService>(sp =>
            new LocalFileStorageService(FileSystem.AppDataDirectory));

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}