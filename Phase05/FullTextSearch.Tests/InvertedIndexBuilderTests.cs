using FluentAssertions;
using FullTextSearch.InvertedIndex.BuilderServices;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.Tests;

public class InvertedIndexBuilderTests
{
    private readonly ITokenizer _tokenizer;
    private readonly InvertedIndexBuilder _sut;

    public InvertedIndexBuilderTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _sut = new InvertedIndexBuilder(_tokenizer);
    }

    private static Dictionary<string, string> CreateTestDocuments()
    {
        return new Dictionary<string, string>
        {
            { "doc1", "apple banana" },
            { "doc2", "banana carrot" },
            { "doc3", "apple carrot" }
        };
    }

    private void SetupTokenizerMocks()
    {
        _tokenizer.Tokenize("apple banana").Returns(new[] { "APPLE", "BANANA" });
        _tokenizer.Tokenize("banana carrot").Returns(new[] { "BANANA", "CARROT" });
        _tokenizer.Tokenize("apple carrot").Returns(new[] { "APPLE", "CARROT" });
    }

    [Fact]
    public void Build_ShouldIncludeAllDocumentIdsInAllDocuments_WhenDocumentsAreProvided()
    {
        // Arrange
        var docs = CreateTestDocuments();
        SetupTokenizerMocks();

        // Act
        var dto = _sut.Build(docs);

        // Assert
        dto.AllDocuments.Should().BeEquivalentTo(docs.Keys);
    }

    [Fact]
    public void Build_ShouldCreateCorrectIndexEntriesForApple_WhenAppleAppearsInMultipleDocuments()
    {
        // Arrange
        var docs = CreateTestDocuments();
        SetupTokenizerMocks();

        var expectedAppleDocs = new HashSet<DocumentInfo>
        {
            new() { DocId = "doc1", Indexes = [0] },
            new() { DocId = "doc3", Indexes = [0] }
        };

        // Act
        var dto = _sut.Build(docs);

        // Assert
        dto.InvertedIndexMap.Should().ContainKey("APPLE")
            .WhoseValue.Should().BeEquivalentTo(expectedAppleDocs);
    }

    [Fact]
    public void Build_ShouldCreateCorrectIndexEntriesForBanana_WhenBananaAppearsInMultiplePositions()
    {
        // Arrange
        var docs = CreateTestDocuments();
        SetupTokenizerMocks();

        var expectedBananaDocs = new HashSet<DocumentInfo>
        {
            new() { DocId = "doc1", Indexes = [1] },
            new() { DocId = "doc2", Indexes = [0] }
        };

        // Act
        var dto = _sut.Build(docs);

        // Assert
        dto.InvertedIndexMap.Should().ContainKey("BANANA")
            .WhoseValue.Should().BeEquivalentTo(expectedBananaDocs);
    }

    [Fact]
    public void Build_ShouldCreateCorrectIndexEntriesForCarrot_WhenCarrotAppearsInMultipleDocuments()
    {
        // Arrange
        var docs = CreateTestDocuments();
        SetupTokenizerMocks();

        var expectedCarrotDocs = new HashSet<DocumentInfo>
        {
            new() { DocId = "doc2", Indexes = [1] },
            new() { DocId = "doc3", Indexes = [1] }
        };

        // Act
        var dto = _sut.Build(docs);

        // Assert
        dto.InvertedIndexMap.Should().ContainKey("CARROT")
            .WhoseValue.Should().BeEquivalentTo(expectedCarrotDocs);
    }

    [Fact]
    public void Build_ShouldCallTokenizeOnceForEachDocument_WhenBuildingIndex()
    {
        // Arrange
        var docs = CreateTestDocuments();
        SetupTokenizerMocks();

        // Act
        _sut.Build(docs);

        // Assert
        _tokenizer.Received(1).Tokenize("apple banana");
        _tokenizer.Received(1).Tokenize("banana carrot");
        _tokenizer.Received(1).Tokenize("apple carrot");
    }

    [Fact]
    public void Build_ShouldCreateEmptyIndex_WhenNoDocumentsAreProvided()
    {
        // Arrange
        var emptyDocs = new Dictionary<string, string>();

        // Act
        var dto = _sut.Build(emptyDocs);

        // Assert
        dto.AllDocuments.Should().BeEmpty();
        dto.InvertedIndexMap.Should().BeEmpty();
    }
}