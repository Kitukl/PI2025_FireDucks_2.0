using Microsoft.VisualBasic.FileIO;

namespace StudyHub.BLL.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(string rootPath)
    {
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new ArgumentException("Root path must be provided.", nameof(rootPath));

        _rootPath = Path.GetFullPath(rootPath);
        Directory.CreateDirectory(_rootPath);
    }

    private string Normalize(string path)
    {
        var full = Path.GetFullPath(Path.Combine(_rootPath, path));

        if (!full.StartsWith(_rootPath, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Access outside of root path is not allowed.");

        return full;
    }

    public Task<IEnumerable<string>> GetFoldersAsync(string path)
    {
        var full = Normalize(path);

        if (!Directory.Exists(full))
            return Task.FromResult(Enumerable.Empty<string>());

        var folders = Directory.GetDirectories(full).Select(Path.GetFileName);
        return Task.FromResult(folders);
    }

    public Task<IEnumerable<string>> GetFilesAsync(string path)
    {
        var full = Normalize(path);

        if (!Directory.Exists(full))
            return Task.FromResult(Enumerable.Empty<string>());

        var files = Directory.GetFiles(full).Select(Path.GetFileName);
        return Task.FromResult(files);
    }

    public Task CreateFolderAsync(string path)
    {
        Directory.CreateDirectory(Normalize(path));
        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(string filePath)
    {
        var full = Normalize(filePath);

        if (File.Exists(full))
            File.Delete(full);

        return Task.CompletedTask;
    }

    public async Task AddFileAsync(string folderPath, Stream fileStream, string fileName)
    {
        var folder = Normalize(folderPath);
        Directory.CreateDirectory(folder);

        var file = Path.Combine(folder, fileName);

        using var dest = File.Create(file);
        await fileStream.CopyToAsync(dest);
    }
}
