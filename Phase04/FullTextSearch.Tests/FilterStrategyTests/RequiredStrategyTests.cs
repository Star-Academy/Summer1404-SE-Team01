using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.FilterStrategies;
using FullTextSearch.InvertedIndex.FilterStrategies.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using NSubstitute;

namespace FullTextSearch.Tests.FilterStrategyTests;

public class RequiredStrategyTests
{
    private readonly ISearch _search;
    private readonly IFilterStrategy _sut;

    public RequiredStrategyTests()
    {
        _search = Substitute.For<ISearch>();
        _sut = new RequiredStrategy(_search);
    }

    private static QueryDto CreateSampleQueryDto()
    {
        return new QueryDto
        {
            Optional = new List<string> { "ILLNESS", "DISEASE" },
            Required = new List<string> { "GET", "HELP" },
            Excluded = new List<string> { "COUGH", "STAR" }
        };
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSearchIsNull()
    {
        // Arrange & Act
        Action act = () => new RequiredStrategy(null);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'searchService')");
    }


    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnIntersectionOfDocuments_WithSearchResults()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3", "doc4", "doc5"],
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1", "doc2"]);
        _search.Search("HELP", dto).Returns(["doc2", "doc3"]);

        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEquivalentTo(["doc2"]);
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenRequiredListEmpty()
    {
        // Arrange
        var queryDto = new QueryDto()
        {
            Required = []
        };

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2"],
            InvertedIndexMap = []

        };


        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEquivalentTo(dto.AllDocuments);
        _search.DidNotReceive().Search(Arg.Any<string>(), Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenAllDocumentsIsEmpty()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = new HashSet<string>(),
            InvertedIndexMap = []

        };

        _search.Search(Arg.Any<string>(), dto).Returns(["doc1"]);

        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void FilterDocumentsByQuery_ShouldReturnEmptySet_WhenNoDocumentsMatchAllRequiredWords()
    {
        // Arrange

        var dto = new InvertedIndexDto
        {
            AllDocuments = ["doc1", "doc2", "doc3"],
            InvertedIndexMap = []

        };

        _search.Search("GET", dto).Returns(["doc1", "doc2"]);
        _search.Search("HELP", dto).Returns(["doc3"]);
        var queryDto = CreateSampleQueryDto();

        // Act
        var expected = _sut.FilterDocumentsByQuery(queryDto, dto);

        // Assert
        expected.Should().BeEmpty();
    }
}