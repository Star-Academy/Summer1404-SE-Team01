using System.Diagnostics.CodeAnalysis;
using FullTextSearch.InvertedIndex.BuilderServices;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using FullTextSearch.Services.QueryBuilder;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch
{
    [ExcludeFromCodeCoverage]
    class Program
    {
        static void Main(string[] args)
        {
            var fileReader = new FileReader();
            var logger = new ConsoleLogger();
            var tokenizer = new Tokenizer();
            var documentAdder = new DocumentAdder(tokenizer);
            var invertedIndex = new InvertedIndexBuilder(documentAdder);

            var sequentialValidator = new SequentialValidator();
            var searchService = new SearchService(tokenizer, sequentialValidator);
            var advanceSearch = new AdvancedSearch();
            var singleWordExtractor = new SingleWordQueryExtractor();
            var phraseExtractor = new PhraseQueryExtractor();
            var app = new SearchApplication(
                fileReader,
                invertedIndex,
                logger,
                searchService,
                advanceSearch,
                singleWordExtractor,
                phraseExtractor);

            app.Run();
        }
    }
}