using Microsoft.VisualBasic.FileIO;

namespace StudyHub.BLL.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(string? rootPath = null)
    {
        _rootPath = rootPath;
    }
    
    public Task<IEnumerable<string>> GetFoldersAsync(string path)
    {
        string fullPath = Path.Combine(_rootPath, path);
        var folders = Directory.GetDirectories(fullPath).Select(Path.GetFileName);
        
        return Task.FromResult(folders);
    }

    public Task<IEnumerable<string>> GetFilesAsync(string path)
    {
        string fullPath = Path.Combine(_rootPath, path);
        var files = Directory.GetFiles(fullPath).Select(Path.GetFileName);
        
        return Task.FromResult(files);
    }

    public Task CreateFolderAsync(string path)
    {
        Directory.CreateDirectory(Path.Combine(_rootPath, path));
        
        return Task.CompletedTask;
    }

    public Task DeleteFileAsync(string filePath)
    {
        File.Delete(Path.Combine(_rootPath, filePath));
        
        return Task.CompletedTask;
    }

    public async Task AddFileAsync(string folderPath, Stream fileStream, string fileName)
    {
        var target = Path.Combine(_rootPath, folderPath, fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        using var dest = File.Create(target);
        await fileStream.CopyToAsync(dest);
    }
}
