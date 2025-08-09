using FluentAssertions;
using FullTextSearch.API.InvertedIndex.BuilderServices;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.API.Tests.BuilderServicesTests;

public class DocumentAdderTests
{
    private readonly ITokenizer _tokenizer;

    public DocumentAdderTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTokenizerIsNull()
    {
        //Act
        Action act = () => new DocumentAdder(null);

        act.Should().ThrowExactly<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'tokenizer')");
    }

    [Fact]
    public void AddDocument_ShouldAddDocumentToPassedInvertedIndexDto_WhenDocIdAndContentsIsProvided()
    {
        // Arrange 
        var docId = "doc1";
        var content = "content of doc1";
        var tokens = new List<string> { "CONTENT", "OF", "DOC1" };
        _tokenizer.Tokenize(content).Returns(tokens);

        var indexDto = new InvertedIndexDto()
        {
            AllDocuments = [],
            InvertedIndexMap = new SortedDictionary<string, HashSet<DocumentInfo>>()
        };

        var sut = new DocumentAdder(_tokenizer);

        // Act
        sut.AddDocument(docId, content, indexDto);


        // Assert
        foreach (var (word, index) in tokens.Select((w, i) => (w, i)))
        {
            indexDto.InvertedIndexMap.Should().ContainKey(word);

            var documentInfos = indexDto.InvertedIndexMap[word];
            documentInfos.Should().ContainSingle(doc => doc.DocId == docId);

            var documentInfo = documentInfos.Single(doc => doc.DocId == docId);
            documentInfo.Indexes.Should().Contain(index);
        }

    }

    [Fact]
    public void AddDocument_ShouldUpdateExistingDocumentInfo_WhenWordOccursMultipleTimes()
    {
        // Arrange
        var docId = "doc1";
        var content = "word word word"; // same word repeated
        var tokens = new List<string> { "WORD", "WORD", "WORD" };

        _tokenizer.Tokenize(content).Returns(tokens);

        var indexDto = new InvertedIndexDto
        {
            AllDocuments = [],
            InvertedIndexMap = new SortedDictionary<string, HashSet<DocumentInfo>>()
        };

        var sut = new DocumentAdder(_tokenizer);

        // Act
        sut.AddDocument(docId, content, indexDto);

        // Assert
        indexDto.InvertedIndexMap.Should().ContainKey("WORD");

        var wordSet = indexDto.InvertedIndexMap["WORD"];
        wordSet.Should().ContainSingle(doc => doc.DocId == docId);

        var docInfo = wordSet.Single(doc => doc.DocId == docId);
        docInfo.Indexes.Should().BeEquivalentTo(new[] { 0, 1, 2 });
    }

}