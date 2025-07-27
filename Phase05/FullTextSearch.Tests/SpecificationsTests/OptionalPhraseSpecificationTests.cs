using FullTextSearch.InvertedIndexDs.QueryBuilder;
using FullTextSearch.InvertedIndexDs.SearchFeatures;
using NSubstitute;

namespace FullTextSearch.Tests.SpecificationsTests
{
    public class OptionalPhraseSpecificationTests
    {
        private readonly ISearch _simpleSearch;
        private readonly IQueryExtractor _queryExtractor;
        private readonly string _query;
        private readonly Dictionary<string, string> _documents;
        public OptionalPhraseSpecificationTests()
        {

            _simpleSearch = Substitute.For<ISearch>();
            _queryExtractor = Substitute.For<IQueryExtractor>();
            _query = "get help +illness +disease -cough +\"star academy\"";
            _documents = new Dictionary<string, string>()
            {
                ["doc1"] = "hello my name is star",
                ["doc2"] = "welcome to my academy",
                ["doc3"] = "this academy is called star academy",
                ["doc4"] = "it is not called start without academy",
                ["doc5"] = "so again I shall say welcome to start academy"
            };

        }

    }
}
