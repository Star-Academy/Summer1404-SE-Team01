using FullTextSearch.Services.LoggerService;

namespace FullTextSearch.Services.FileReaderService
{
    
    public class FileReader : IFileReader
    {
        private readonly ILogger _logger;

        public FileReader(ILogger logger)
        {
            _logger = logger;
        }

        public Dictionary<string, string> ReadAllFiles(string basePath)
        {
            var docs = new Dictionary<string, string>();
            
            if (!Directory.Exists(basePath))
            {
                _logger.LogWarning($"Directory does not exist: {basePath}");
                return docs;
            }

            try
            {
                foreach (string filePath in Directory.GetFiles(basePath))
                {
                    string content = File.ReadAllText(filePath);
                    string docId = Path.GetFileName(filePath);
                    docs[docId] = content;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reading files from {basePath}: {ex.Message}");
            }

            return docs;
        }
    }
}