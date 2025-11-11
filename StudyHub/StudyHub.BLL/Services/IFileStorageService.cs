namespace StudyHub.BLL.Services;

public interface IFileStorageService
{
    Task<IEnumerable<string>> GetFoldersAsync(string path);
    Task<IEnumerable<string>> GetFilesAsync(string path);
    Task CreateFolderAsync(string path);
    Task DeleteFileAsync(string filePath);
    Task AddFileAsync(string folderPath, Stream fileStream, string fileName);
}
