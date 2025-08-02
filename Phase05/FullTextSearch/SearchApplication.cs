using FullTextSearch.InvertedIndex;
using FullTextSearch.InvertedIndex.Constants;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using System.Diagnostics.CodeAnalysis;

namespace FullTextSearch;

[ExcludeFromCodeCoverage]
public class SearchApplication
{
    private const string DataSetPath = "EnglishData";
    private readonly IFileReader _fileReader;
    private readonly IInvertedIndexBuilder _invertedIndex;
    private readonly ILogger _logger;
    private readonly ISearch _simpleSearch;
    private readonly ISearch _phraseSearch;
    private readonly IQueryExtractor _queryExtractor;
    private readonly IQueryExtractor _phraseQueryExtractor;

    public SearchApplication(
        IFileReader fileReader,
        IInvertedIndexBuilder invertedIndex,
        ILogger logger,
        ISearch simpleSearch,
        IQueryExtractor queryExtractor,
        ISearch phraseSearch,
        IQueryExtractor phraseQueryExtractor)
    {
        _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
        _invertedIndex = invertedIndex ?? throw new ArgumentNullException(nameof(invertedIndex));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _simpleSearch = simpleSearch ?? throw new ArgumentNullException(nameof(simpleSearch));
        _queryExtractor = queryExtractor ?? throw new ArgumentNullException(nameof(queryExtractor));
        _phraseSearch = phraseSearch ?? throw new ArgumentNullException(nameof(phraseSearch));
        _phraseQueryExtractor = phraseQueryExtractor ?? throw new ArgumentNullException(nameof(phraseQueryExtractor));
    }

    public void Run()
    {
        try
        {
            _logger.LogInformation("Initializing search application...");
            var dto = InitializeIndex();
            if (dto is null) throw new ArgumentNullException($"InvertedIndex {nameof(dto)} is null)");
            RunMainLoop(dto);
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

    private InvertedIndexDto InitializeIndex()
    {
        _logger.LogInformation($"Loading documents from '{DataSetPath}'...");
        try
        {
            var docs = _fileReader.ReadAllFiles(DataSetPath);
            _logger.LogInformation("Building inverted index...");
            var dto = _invertedIndex.Build(docs);

            _logger.LogInformation($"Index built successfully. {docs.Count} documents loaded.");
            return dto;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            Environment.Exit(1);
        }

        return null;
    }

    private void RunMainLoop(InvertedIndexDto dto)
    {
        while (true)
        {
            DisplayMenu();
            var choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "q")
            {
                break;
            }

            ProcessUserChoice(choice, dto);
        }
    }

    private void DisplayMenu()
    {
        Console.WriteLine("\nChoose search mode:");
        Console.WriteLine("1. Single word search");
        Console.WriteLine("2. Advanced query search (supports +required -excluded words)");
        Console.WriteLine("q. Quit");
        Console.Write("Your choice: ");
    }

    private void ProcessUserChoice(string choice, InvertedIndexDto dto)
    {
        switch (choice)
        {
            case "1":
                RunSearchLoop("word", dto, SearchSingleWord);
                break;
            case "2":
                RunAdvancedSearchLoop(dto);
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }

    private void RunSearchLoop(string searchType, InvertedIndexDto dto, Func<string, InvertedIndexDto, IEnumerable<string>> searchFunction)
    {
        while (true)
        {
            Console.WriteLine($"\nEnter a {searchType} to search (or 'q' to return to menu):");
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim();

            if (input?.ToLower() == "q") break;

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Input cannot be empty");
                continue;
            }

            try
            {
                var results = searchFunction(input, dto);
                DisplayResults(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Search error: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void RunAdvancedSearchLoop(InvertedIndexDto dto)
    {
        while (true)
        {
            Console.WriteLine("\nEnter an advanced query to search (or 'q' to return to menu):");
            Console.WriteLine("Example: +apple -banana orange");
            Console.Write("> ");
            var query = Console.ReadLine()?.Trim();

            if (query?.ToLower() == "q") break;

            if (string.IsNullOrWhiteSpace(query))
            {
                Console.WriteLine("Input cannot be empty");
                continue;
            }

            try
            {
                var strategies = CreateFilterStrategies();

                var advancedSearch = new AdvancedSearch(strategies);

                var results = advancedSearch.Search(query, dto);
                DisplayResults(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Advanced search error: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private List<IFilterStrategy> CreateFilterStrategies()
    {
        return new List<IFilterStrategy>
        {
            new RequiredStrategy(_simpleSearch, _queryExtractor, StrategyPatterns.RequiredSingleWord),
            new RequiredStrategy(_phraseSearch, _phraseQueryExtractor, StrategyPatterns.RequiredPhrase),
            new OptionalStrategy(_simpleSearch, _queryExtractor, StrategyPatterns.OptionalSingleWord),
            new OptionalStrategy(_phraseSearch, _phraseQueryExtractor, StrategyPatterns.OptionalPhrase),
            new ExcludedStrategy(_simpleSearch, _queryExtractor, StrategyPatterns.ExcludedSingleWord),
            new ExcludedStrategy(_phraseSearch, _phraseQueryExtractor, StrategyPatterns.ExcludedPhrase)
        };
    }

    private IEnumerable<string> SearchSingleWord(string word, InvertedIndexDto dto)
    {
        return _simpleSearch.Search(word.ToUpper(), dto);
    }

    private void DisplayResults(IEnumerable<string> results)
    {
        var resultsList = results.ToList();

        if (!resultsList.Any())
        {
            Console.WriteLine("No documents found matching your search.");
            return;
        }

        Console.WriteLine($"\nFound {resultsList.Count} documents:");
        foreach (var (doc, index) in resultsList.Select((doc, index) => (doc, index + 1)))
        {
            Console.WriteLine($"{index}. {doc}");
        }
    }
}