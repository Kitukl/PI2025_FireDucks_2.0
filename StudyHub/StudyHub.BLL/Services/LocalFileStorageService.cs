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

    private string GetUserRootPath(int userId)
    {
        var userPath = Path.Combine(_rootPath, $"user_{userId}");
        Directory.CreateDirectory(userPath);
        return userPath;
    }

    private string Normalize(int userId, string path)
    {
        var userRoot = GetUserRootPath(userId);
        var full = Path.GetFullPath(Path.Combine(userRoot, path));

        if (!full.StartsWith(userRoot, StringComparison.Ordinal))
            throw new UnauthorizedAccessException("Access outside of user storage is not allowed.");

        return full;
    }

    public Task<IEnumerable<string>> GetFoldersAsync(int userId, string path)
    {
        var full = Normalize(userId, path);
        if (!Directory.Exists(full))
            return Task.FromResult(Enumerable.Empty<string>());

        var folders = Directory.GetDirectories(full).Select(Path.GetFileName);
        return Task.FromResult(folders);
    }

    public Task<IEnumerable<string>> GetFilesAsync(int userId, string path)
    {
        var full = Normalize(userId, path);
        if (!Directory.Exists(full))
            return Task.FromResult(Enumerable.Empty<string>());

        var files = Directory.GetFiles(full).Select(Path.GetFileName);
        return Task.FromResult(files);
    }

    public Task CreateFolderAsync(int userId, string path)
    {
        Directory.CreateDirectory(Normalize(userId, path));
        return Task.CompletedTask;
    }

    public Task DeleteFolderAsync(int userId, string folderPath)
    {
        var full = Normalize(userId, folderPath);
        if (Directory.Exists(full))
            Directory.Delete(full, recursive: true); 
        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(int userId, string filePath)
    {
        var full = Normalize(userId, filePath);
        if (File.Exists(full))
            File.Delete(full);
        return Task.CompletedTask;
    }

    public async Task AddFileAsync(int userId, string folderPath, Stream fileStream, string fileName)
    {
        var folder = Normalize(userId, folderPath);
        Directory.CreateDirectory(folder);
        var file = Path.Combine(folder, fileName);

        using var dest = File.Create(file);
        await fileStream.CopyToAsync(dest);
    }
}