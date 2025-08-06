using FluentAssertions;
using FullTextSearch.API.InvertedIndex.BuilderServices;
using FullTextSearch.API.InvertedIndex.BuilderServices.Abstraction;
using FullTextSearch.API.InvertedIndex.Dtos;
using NSubstitute;

namespace FullTextSearch.API.Tests.BuilderServicesTests;

public class InvertedIndexBuilderTests
{
    
    private readonly InvertedIndexBuilder _sut;
    private readonly IDocumentAdder _documentAdder;
    public InvertedIndexBuilderTests()
    {
        _documentAdder = Substitute.For<IDocumentAdder>();
        _sut = new InvertedIndexBuilder(_documentAdder);
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

    [Fact]
    public void Constructor_ShouldThrowException_WhenDocumentAdderIsNull()
    {
        Action act = () => new InvertedIndexBuilder(null);

        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'documentAdder')");
    }
    
    [Fact]
    public void Build_ShouldIncludeAllDocumentIdsInAllDocuments_WhenDocumentsAreProvided()
    {
        // Arrange
        var docs = CreateTestDocuments();

        // Act
        var dto = _sut.Build(docs);

        // Assert
        dto.AllDocuments.Should().BeEquivalentTo(docs.Keys);
    }

    [Fact]
    public void Build_ShouldCallAddDocument3times_WhenProvidedExactly3Documents()
    {
        // Arrange
        var docs = CreateTestDocuments();

        // Act
        var dto = _sut.Build(docs);

        // Assert
        _documentAdder.Received(3).AddDocument(Arg.Any<string>(), Arg.Any<string>(), dto);
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