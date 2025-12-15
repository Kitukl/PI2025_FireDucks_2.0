namespace StudyHub.BLL.Services;

public interface IFileStorageService
{
    Task<IEnumerable<string>> GetFoldersAsync(int userId, string path);
    Task<IEnumerable<string>> GetFilesAsync(int userId, string path);
    Task CreateFolderAsync(int userId, string path);
    Task DeleteFolderAsync(int userId, string folderPath);
    Task DeleteFileAsync(int userId, string filePath);
    Task AddFileAsync(int userId, string folderPath, Stream fileStream, string fileName);
}
