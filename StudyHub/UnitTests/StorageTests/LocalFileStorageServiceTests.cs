using StudyHub.BLL.Services;

namespace StudyHub.UnitTests;

public class LocalFileStorageServiceTests : IDisposable
{
    private readonly string _tempRoot;
    private readonly LocalFileStorageService _service;

    public LocalFileStorageServiceTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempRoot);

        _service = new LocalFileStorageService(_tempRoot);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }

    // -------------------------
    //       GET FOLDERS
    // -------------------------

    [Fact]
    public async Task GetFoldersAsync_ReturnsFolders()
    {
        string[] expectedFolders = { "Folder1", "Folder2", "Folder3" };
        foreach (var folder in expectedFolders)
        {
            Directory.CreateDirectory(Path.Combine(_tempRoot, folder));
        }

        // Pass RELATIVE path 
        var actualFolders = await _service.GetFoldersAsync("");

        Assert.Equal(
            expectedFolders.OrderBy(x => x),
            actualFolders.OrderBy(x => x));
    }

    [Fact]
    public async Task GetFoldersAsync_ReturnsEmpty_WhenDirectoryDoesNotExist()
    {
        var folders = await _service.GetFoldersAsync("missing");

        Assert.Empty(folders);
    }

    // -------------------------
    //       GET FILES
    // -------------------------

    [Fact]
    public async Task GetFilesAsync_ReturnsFiles()
    {
        string folder = "MyFolder";
        string fileName = "file.txt";

        Directory.CreateDirectory(Path.Combine(_tempRoot, folder));
        File.WriteAllText(Path.Combine(_tempRoot, folder, fileName), "x");

        var files = await _service.GetFilesAsync(folder);

        Assert.Contains(fileName, files);
    }

    [Fact]
    public async Task GetFilesAsync_ReturnsEmpty_WhenDirectoryMissing()
    {
        var files = await _service.GetFilesAsync("missing");

        Assert.Empty(files);
    }

    // -------------------------
    //     CREATE FOLDER
    // -------------------------

    [Fact]
    public async Task CreateFolderAsync_CreatesFolder()
    {
        string folderName = "TestFolder";

        await _service.CreateFolderAsync(folderName);

        Assert.True(Directory.Exists(Path.Combine(_tempRoot, folderName)));
    }

    // -------------------------
    //      DELETE FILE
    // -------------------------

    [Fact]
    public async Task DeleteFileAsync_RemovesFile()
    {
        string file = "delete.txt";
        string fullPath = Path.Combine(_tempRoot, file);

        File.WriteAllText(fullPath, "temp");

        await _service.DeleteFileAsync(file);

        Assert.False(File.Exists(fullPath));
    }

    [Fact]
    public async Task DeleteFileAsync_DoesNothing_WhenFileMissing()
    {
        await _service.DeleteFileAsync("missing.txt");

        // no exception means success
        Assert.True(true);
    }

    // -------------------------
    //      ADD FILE
    // -------------------------

    [Fact]
    public async Task AddFileAsync_CreatesFileWithContent()
    {
        string folder = "Docs";
        string fileName = "hello.txt";

        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hello"));

        await _service.AddFileAsync(folder, stream, fileName);

        string filePath = Path.Combine(_tempRoot, folder, fileName);

        Assert.True(File.Exists(filePath));
        Assert.Equal("hello", File.ReadAllText(filePath));
    }

    // -------------------------
    //      SECURITY TESTS
    // -------------------------

    [Fact]
    public async Task Operations_Throw_WhenPathEscapesRoot()
    {
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.GetFilesAsync("../outside"));
    }

    [Fact]
    public async Task AddFileAsync_Throws_WhenEscapingRoot()
    {
        using var s = new MemoryStream(new byte[] { 1 });

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _service.AddFileAsync("../hack", s, "x.txt"));
    }
}
