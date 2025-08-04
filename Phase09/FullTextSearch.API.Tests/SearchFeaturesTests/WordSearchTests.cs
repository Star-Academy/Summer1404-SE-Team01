using FluentAssertions;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.InvertedIndex.SearchFeatures;

namespace FullTextSearch.API.Tests.SearchFeaturesTests;

public class WordSearchTests
{
    private readonly ISearch _sut;

    public WordSearchTests()
    {
        _sut = new WordSearch();
    }

    [Fact]
    public void Search_ShouldReturnMatchingDocuments_WhenWordExists()
    {
        // Arrange
        var dtoWithAppleDocs = CreateTestDtoWithAppleDocuments();
        var expectedAppleDocIds = new SortedSet<string> { "doc1", "doc2" };

        // Act
        var expected = _sut.Search("apple", dtoWithAppleDocs);

        // Assert
        expected.Should().BeEquivalentTo(expectedAppleDocIds);
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenWordNotExists()
    {
        // Arrange
        var dtoWithAppleDocs = CreateTestDtoWithAppleDocuments();

        // Act
        var expected = _sut.Search("orange", dtoWithAppleDocs);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Search_ShouldBeCaseInsensitive()
    {
        // Arrange
        var dtoWithAppleDocs = CreateTestDtoWithAppleDocuments();
        var expectedAppleDocIds = new SortedSet<string> { "doc1", "doc2" };

        // Act
        var expected = _sut.Search("ApPLe", dtoWithAppleDocs);

        // Assert
        expected.Should().BeEquivalentTo(expectedAppleDocIds);
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenDtoIsEmpty()
    {
        // Arrange
        var emptyDto = new InvertedIndexDto
        {
            AllDocuments = new(),
            InvertedIndexMap = new()
        };

        // Act
        var expected = _sut.Search("apple", emptyDto);

        // Assert
        expected.Should().BeEmpty();
    }

    private static InvertedIndexDto CreateTestDtoWithAppleDocuments()
    {
        var appleDocInfos = new SortedSet<DocumentInfo>
        {
            new DocumentInfo
            {
                DocId = "doc1",
                Indexes = [50, 43, 44]
            },
            new DocumentInfo
            {
                DocId = "doc2",
                Indexes = [51, 99, 90]
            }
        };

        return new InvertedIndexDto
        {
            AllDocuments = new(),
            InvertedIndexMap = new()
            {
                ["APPLE"] = appleDocInfos
            }
        };
    }
}