using Microsoft.Playwright;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace StudyHub.BLL.Services;


public interface IFacultyHelper
{
    string BaseUrl { get; }
    string GetLinkSearchCriteria(string groupName);
    bool IsMatch(string groupName);
}


public class AmiFacultyHelper : IFacultyHelper
{
    public string BaseUrl => "https://ami.lnu.edu.ua/students/rozklad-zanyat";

    private readonly Dictionary<string, string> _specializationMap = new()
    {
        { "ПМІ", "КН" },
        { "ПМО", "СО" },
        { "ПМП", "ПМ" },
        { "ПМК", "КБ" },
        { "ПМА", "СА" }
    };

    public string GetLinkSearchCriteria(string groupName)
    {
        var specializationMatch = Regex.Match(groupName, @"^([А-ЯІЇЄ]+)");
        var courseMatch = Regex.Match(groupName, @"\d+");

        if (!specializationMatch.Success || !courseMatch.Success)
        {
            throw new FormatException($"Не вдалося розпізнати формат групи: '{groupName}'. Очікуваний формат: 'ПМІ-31с'.");
        }

        string prefix = specializationMatch.Value;
        string numberPart = courseMatch.Value;

        char courseNumber = numberPart[0];

        if (!_specializationMap.TryGetValue(prefix, out var siteIdentifier))
        {
            siteIdentifier = "";
        }

        return $"{courseNumber} курс|{siteIdentifier}";
    }

    public bool IsMatch(string groupName)
    {
        return !string.IsNullOrWhiteSpace(groupName) && groupName.StartsWith("ПМ", StringComparison.OrdinalIgnoreCase);
    }
}


public interface IScheduleDownloaderService
{
    Task<string> DownloadSchedulePdfAsync(IFacultyHelper faculty, string groupName);
}

public class ScheduleDownloaderService : IScheduleDownloaderService
{
    public async Task<string> DownloadSchedulePdfAsync(IFacultyHelper faculty, string groupName)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException("Playwright працює тільки на Windows.");
        }

        string searchCriteria = faculty.GetLinkSearchCriteria(groupName);
        var criteriaParts = searchCriteria.Split('|');
        string coursePart = criteriaParts[0];
        string specPart = criteriaParts.Length > 1 ? criteriaParts[1] : "";

        using var playwright = await Playwright.CreateAsync();

        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await browser.NewContextAsync(new BrowserNewContextOptions { AcceptDownloads = true });
        var page = await context.NewPageAsync();

        await page.GotoAsync(faculty.BaseUrl);

        var downloadTask = page.WaitForDownloadAsync();

        ILocator linkLocator;

        if (!string.IsNullOrEmpty(specPart))
        {
            linkLocator = page.Locator($"a[href$='.pdf']")
                              .Filter(new LocatorFilterOptions { HasText = coursePart })
                              .Filter(new LocatorFilterOptions { HasText = specPart });
        }
        else
        {
            linkLocator = page.Locator($"a[href$='.pdf']")
                              .Filter(new LocatorFilterOptions { HasText = coursePart });
        }

        if (await linkLocator.CountAsync() == 0)
        {
            throw new FileNotFoundException($"Не знайдено PDF-файл розкладу на сайті для групи {groupName}. (Критерії пошуку: {coursePart} + {specPart})");
        }

        await linkLocator.First.ClickAsync();

        var download = await downloadTask;

        var folder = Path.Combine(AppContext.BaseDirectory, "Resources", "Schedules");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

        var safeGroupName = string.Join("_", groupName.Split(Path.GetInvalidFileNameChars()));
        var fileName = $"Schedule_{safeGroupName}_{DateTime.Now:yyyyMMdd_HHmm}.pdf";

        var filePath = Path.Combine(folder, fileName);

        await download.SaveAsAsync(filePath);

        return filePath;
    }
}