using FullTextSearch.InvertedIndex.FilterSpecifications;
using FullTextSearch.InvertedIndex.QueryBuilder;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch;

class Program
{
    static void Main(string[] args)
    {
        var logger = new ConsoleLogger();
        var tokenizer = new Tokenizer();
        var fileReader = new FileReader(logger);
        var invertedIndex = new InvertedIndex.InvertedIndex(tokenizer);

        var queryExtractor = new QueryExtractor();

        var simpleSearch = new InvertedIndexSimpleSearch(invertedIndex);

        var necessarySpecification = new NecessarySpecification(simpleSearch);
        var optionalSpecification = new OptionalSpecification(simpleSearch);
        var excludedSpecification = new ExcludedSpecification(simpleSearch);


        var advancedSearch = new InvertedIndexAdvancedSearch(
            invertedIndex,
            queryExtractor,
            simpleSearch,
            necessarySpecification,
            optionalSpecification,
            excludedSpecification);

        var app = new SearchApplication(
            fileReader,
            invertedIndex,
            logger,
            simpleSearch,
            advancedSearch);

        app.Run();
    }
}