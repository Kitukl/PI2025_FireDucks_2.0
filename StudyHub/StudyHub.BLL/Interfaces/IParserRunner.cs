namespace StudyHub.BLL.Interfaces;

public interface IParserRunner
{
    Task<string> RunParserAsync(string pdfPath);
}