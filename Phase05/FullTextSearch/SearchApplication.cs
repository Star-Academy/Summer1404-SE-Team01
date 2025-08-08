using System.Diagnostics.CodeAnalysis;
using FullTextSearch.InvertedIndex.BuilderServices.Abstractions;
using FullTextSearch.InvertedIndex.Constants;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.QueryBuilder.Abstractions;

namespace FullTextSearch;

[ExcludeFromCodeCoverage]
public class SearchApplication
{
    private const string DataSetPath = "EnglishData";

    private readonly IFileReader _fileReader;
    private readonly IInvertedIndexBuilder _invertedIndex;
    private readonly ILogger _logger;
    private readonly ISearch _searchService;
    private readonly IAdvancedSearch _advancedSearch;
    private readonly IQueryExtractor _wordExtractor;
    private readonly IQueryExtractor _phraseExtractor;

    public SearchApplication(IFileReader fileReader,
        IInvertedIndexBuilder invertedIndex,
        ILogger logger,
        ISearch searchService,
        IAdvancedSearch advancedSearch,
        IQueryExtractor wordExtractor,
        IQueryExtractor phraseExtractor)
    {
        _fileReader = fileReader;
        _invertedIndex = invertedIndex;
        _logger = logger;
        _searchService = searchService;
        _advancedSearch = advancedSearch;
        _wordExtractor = wordExtractor;
        _phraseExtractor = phraseExtractor;
    }

    public void Run()
    {
        try
        {
            _logger.LogInformation("Initializing search application...");
            var invIdxDto = InitializeIndex();
            if (invIdxDto is null) throw new ArgumentNullException($"InvertedIndex {nameof(invIdxDto)} is null)");
            RunMainLoop(invIdxDto);
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
            var invIdxDto = _invertedIndex.Build(docs);

            _logger.LogInformation($"Index built successfully. {docs.Count} documents loaded.");
            return invIdxDto;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            Environment.Exit(1);
        }

        return null;
    }

    private void RunMainLoop(InvertedIndexDto invIdxDto)
    {
        while (true)
        {
            DisplayMenu();
            var choice = Console.ReadLine()?.Trim().ToLower();

            if (choice == "q")
            {
                break;
            }

            ProcessUserChoice(choice, invIdxDto);
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

    private void ProcessUserChoice(string choice, InvertedIndexDto invIdxDto)
    {
        switch (choice)
        {
            case "1":
                RunSearchLoop(invIdxDto, SearchSingleWord);
                break;
            case "2":
                RunAdvancedSearchLoop(invIdxDto);
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                break;
        }
    }

    private void RunSearchLoop(InvertedIndexDto invIdxDto, Func<string, InvertedIndexDto, IEnumerable<string>> searchFunction)
    {
        while (true)
        {
            Console.WriteLine($"\nEnter a word to search (or 'q' to return to menu):");
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
                var results = searchFunction(input, invIdxDto);
                DisplayResults(results);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Search error: {ex.Message}");
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private void RunAdvancedSearchLoop(InvertedIndexDto invIdxDto)
    {

        while (true)
        {
            Console.WriteLine("\nEnter a query to search (or 'q' to return to menu):");
            Console.WriteLine("Example: +apple -banana orange");
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
                var strategies = CreateFilterStrategies();

                var queryDto = CreateQueryDto(input);

                var results = _advancedSearch.Search(queryDto, invIdxDto, strategies);
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
            new RequiredStrategy(_searchService),
            new OptionalStrategy(_searchService),
            new ExcludedStrategy(_searchService),
        };
    }

    private QueryDto CreateQueryDto(string input)
    {
        var queryDto = new QueryDto();
        var requiredWords = _wordExtractor.ExtractQueries(input, StrategyPatterns.RequiredSingleWord);
        var requiredPhrases = _phraseExtractor.ExtractQueries(input, StrategyPatterns.RequiredPhrase);
        var optionalWords = _wordExtractor.ExtractQueries(input, StrategyPatterns.OptionalSingleWord);
        var optionalPhrases = _phraseExtractor.ExtractQueries(input, StrategyPatterns.OptionalPhrase);
        var excludedWords = _wordExtractor.ExtractQueries(input, StrategyPatterns.ExcludedSingleWord);
        var excludedPhrases = _phraseExtractor.ExtractQueries(input, StrategyPatterns.ExcludedPhrase);
        queryDto.Required = new([.. requiredWords, .. requiredPhrases]);
        queryDto.Optional = new([.. optionalWords, .. optionalPhrases]);
        queryDto.Excluded = new([.. excludedWords, .. excludedPhrases]);

        return queryDto;
    }

    private IEnumerable<string> SearchSingleWord(string input, InvertedIndexDto invIdxDto)
    {
        return _searchService.Search(input, invIdxDto);
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