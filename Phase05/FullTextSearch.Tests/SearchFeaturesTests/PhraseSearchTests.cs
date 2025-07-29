using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.Tests.SearchFeaturesTests
{
    public class PhraseSearchTests
    {
        private readonly ITokenizer _tokenizer;
        private readonly PhraseSearch _phraseSearch;

        public PhraseSearchTests()
        {
            _tokenizer = Substitute.For<ITokenizer>();
            _phraseSearch = new PhraseSearch(_tokenizer);
        }

        [Fact]
        public void Search_ShouldReturnDocumentsContainingExactPhrase()
        {
            // Arrange
            var inputPhrase = "code star";
            _tokenizer.Tokenize(inputPhrase).Returns(new List<string> { "CODE", "STAR" });

            var dto = new InvertedIndexDto
            {
                AllDocuments = new SortedSet<string> { "doc1", "doc2", "doc3" },
                InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>()
            };

            // doc1 and doc2 contain the phrase at positions [(0,1)]
            dto.InvertedIndexMap["CODE"] = new SortedSet<DocumentInfo>
            {
                new DocumentInfo { DocId = "doc1", Indexes = new SortedSet<long> { 0 } },
                new DocumentInfo { DocId = "doc2", Indexes = new SortedSet<long> { 2 } }
            };
            dto.InvertedIndexMap["STAR"] = new SortedSet<DocumentInfo>
            {
                new DocumentInfo { DocId = "doc1", Indexes = new SortedSet<long> { 1 } },
                new DocumentInfo { DocId = "doc2", Indexes = new SortedSet<long> { 3 } },
                new DocumentInfo { DocId = "doc3", Indexes = new SortedSet<long> { 5 } }
            };

            // Act
            var result = _phraseSearch.Search(inputPhrase, dto);

            // Assert
            result.Should().BeEquivalentTo(new[] { "doc1", "doc2" });
        }

        [Fact]
        public void Search_ShouldReturnEmpty_WhenNoExactPhraseMatchAcrossDocuments()
        {
            // Arrange
            var inputPhrase = "code star";
            _tokenizer.Tokenize(inputPhrase).Returns(new List<string> { "CODE", "STAR" });

            var dto = new InvertedIndexDto
            {
                AllDocuments = new SortedSet<string> { "doc1", "doc2" },
                InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>()
            };

            // doc1 has CODE at 0 but STAR at 2 (no consecutive)
            dto.InvertedIndexMap["CODE"] = new SortedSet<DocumentInfo>
            {
                new DocumentInfo { DocId = "doc1", Indexes = new SortedSet<long> { 0 } }
            };
            dto.InvertedIndexMap["STAR"] = new SortedSet<DocumentInfo>
            {
                new DocumentInfo { DocId = "doc1", Indexes = new SortedSet<long> { 2 } }
            };

            // Act
            var result = _phraseSearch.Search(inputPhrase, dto);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Search_ShouldReturnEmpty_WhenAnyWordMissingFromIndex()
        {
            // Arrange
            var inputPhrase = "code star";
            _tokenizer.Tokenize(inputPhrase).Returns(new List<string> { "CODE", "STAR" });

            var dto = new InvertedIndexDto
            {
                AllDocuments = new SortedSet<string> { "doc1" },
                InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>()
            };

            // Only CODE present
            dto.InvertedIndexMap["CODE"] = new SortedSet<DocumentInfo>
            {
                new DocumentInfo { DocId = "doc1", Indexes = new SortedSet<long> { 0 } }
            };
            // STAR missing

            // Act
            var result = _phraseSearch.Search(inputPhrase, dto);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
