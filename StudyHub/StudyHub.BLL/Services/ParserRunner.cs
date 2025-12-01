using System.Diagnostics;
using StudyHub.BLL.Interfaces;

namespace StudyHub.BLL.Services;

public class ParserRunner : IParserRunner
{
    public async Task<string> RunParserAsync(string pdfPath)
    {
        var parserPath = Path.Combine(AppContext.BaseDirectory, "Parsers", "schedule_parser.exe");

        var psi = new ProcessStartInfo
        {
            FileName = parserPath,
            Arguments = $"\"{pdfPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null) throw new InvalidOperationException("Не вдалося запустити процес парсера.");

        string output = await process.StandardOutput.ReadToEndAsync();
        string error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(error))
            throw new InvalidOperationException($"Помилка виконання парсера: {error}");

        if (string.IsNullOrWhiteSpace(output))
            throw new InvalidDataException("Парсер не повернув JSON!");

        return output;
    }
}