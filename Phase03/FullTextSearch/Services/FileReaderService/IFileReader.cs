namespace FullTextSearch.Services.FileReaderService;

public interface IFileReader
{
    Dictionary<string, string> ReadAllFiles(string basePath);
}