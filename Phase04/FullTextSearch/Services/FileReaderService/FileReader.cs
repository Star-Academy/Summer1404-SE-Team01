using FullTextSearch.Exceptions;
using System.Diagnostics.CodeAnalysis;

namespace FullTextSearch.Services.FileReaderService
{
    [ExcludeFromCodeCoverage]
    public class FileReader : IFileReader
    {
        public Dictionary<string, string> ReadAllFiles(string basePath)
        {
            if (!Directory.Exists(basePath))
                throw new DirectoryNotFoundException($"Directory {basePath} not found!");

            var filePaths = Directory.GetFiles(basePath);
            if (filePaths.Length == 0)
                throw new EmptyDirectoryException($"Directory {basePath} is empty.");

            var docs = new Dictionary<string, string>();

            foreach (string filePath in filePaths)
            {
                string content = File.ReadAllText(filePath);
                string docId = Path.GetFileName(filePath);
                docs[docId] = content;
            }

            return docs;
        }
    }
}
