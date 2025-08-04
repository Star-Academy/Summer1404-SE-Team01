namespace FullTextSearch.API.Services.FileReaderService;

public interface IFileReader
{
    Dictionary<string, string> ReadAllFiles(string basePath);
}