using FullTextSearch.InvertedIndex;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;

namespace FullTextSearch
{
    public class SearchApplication
    {
        private readonly IFileReader _fileReader;
        private readonly IInvertedIndex _invertedIndex;
        private readonly ILogger _logger;

        public SearchApplication(IFileReader fileReader, IInvertedIndex invertedIndex, ILogger logger)
        {
            
            _fileReader = fileReader;
            _invertedIndex = invertedIndex;
            _logger = logger;
        }

        public void Run()
        {
            try
            {
                InitializeIndex();
                RunMainLoop();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Application error: {ex.Message}");
            }
            finally
            {
                _logger.LogInformation("Goodbye!");
            }
        }

        private void InitializeIndex()
        {
            string baseDir = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.FullName;
            string englishDataDir = Path.Combine(baseDir, "EnglishData");
            
            var docs = _fileReader.ReadAllFiles(englishDataDir);
            _invertedIndex.BuildIndex(docs);
        }

        private void RunMainLoop()
        {
            while (true)
            {
                DisplayMenu();
                var choice = Console.ReadLine()?.Trim().ToLower();

                if (choice == "q") break;

                ProcessUserChoice(choice);
            }
        }

        private void DisplayMenu()
        {
            Console.WriteLine("\nChoose search mode:");
            Console.WriteLine("1. Single word search");
            Console.WriteLine("2. Query search");
            Console.WriteLine("q. Quit");
            Console.Write("Your choice: ");
        }

        private void ProcessUserChoice(string choice)
        {
            switch (choice)
            {
                case "1":
                    RunSearchLoop("word", _invertedIndex.SearchWord);
                    break;
                case "2":
                    RunSearchLoop("query", _invertedIndex.AdvancedSearch);
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }

        private void RunSearchLoop(string searchType, Func<string, IEnumerable<string>> searchFunction)
        {
            while (true)
            {
                Console.WriteLine($"Please enter a {searchType} to search for (or 'q' to return to main menu):");
                var input = Console.ReadLine()?.Trim();

                if (input?.ToLower() == "q") break;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Input cannot be empty");
                    continue;
                }

                try
                {
                    var results = searchFunction(input.ToUpper());
                    Console.WriteLine(string.Join(", ", results));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Search error: {ex.Message}");
                }
            }
        }
    }
}
