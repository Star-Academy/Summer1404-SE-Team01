using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;
using FullTextSearch.Services.TokenizerService;
using NSubstitute;

namespace FullTextSearch.Tests.SearchFeaturesTests;

public class SearchServiceTests
{
    private readonly ITokenizer _tokenizer;
    private readonly ISequentialPhraseFinder _sequentialValidator;
    private readonly SearchService _sut;

    public SearchServiceTests()
    {
        _tokenizer = Substitute.For<ITokenizer>();
        _sequentialValidator = Substitute.For<ISequentialPhraseFinder>();
        _sut = new SearchService(_tokenizer, _sequentialValidator);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenTokenizerIsNull()
    {
        Action act = () => new SearchService(null, _sequentialValidator);
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'tokenizer')");
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenSequentialValidatorIsNull()
    {
        Action act = () => new SearchService(_tokenizer, null);
        act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'sequentialValidator')");
    }

    [Fact]
    public void Search_ShouldReturnDocumentsContainingExactPhrase_WhenAllWordsExistInSequence()
    {
        // Arrange
        var phrase = "hello world";
        _tokenizer.Tokenize(phrase).Returns(new[] { "HELLO", "WORLD" });

        var dto = CreateTestIndexDto();
        _sequentialValidator.FindSequentialPhrase(
            Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "HELLO", "WORLD" })),
            Arg.Is<HashSet<string>>(x => x.SetEquals(new[] { "doc1", "doc2", "doc3" })),
            Arg.Any<InvertedIndexDto>())
            .Returns(new HashSet<string>(new[] { "doc1", "doc3" }));

        // Act
        var result = _sut.Search(phrase, dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new[] { "doc1", "doc3" });
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenFirstWordIsMissingFromIndex()
    {
        // Arrange
        var phrase = "missing world";
        _tokenizer.Tokenize(phrase).Returns(["MISSING", "WORLD"]);
        var dto = CreateTestIndexDto();

        // Act
        var expected = _sut.Search(phrase, dto);

        // Assert
        expected.Should().BeEmpty();
        _sequentialValidator.DidNotReceive().FindSequentialPhrase(
            Arg.Any<List<string>>(),
            Arg.Any<HashSet<string>>(),
            Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenSubsequentWordIsMissingFromIndex()
    {
        // Arrange
        var phrase = "hello missing";
        _tokenizer.Tokenize(phrase).Returns(["HELLO", "MISSING"]);
        var dto = CreateTestIndexDto();

        // Act
        var expected = _sut.Search(phrase, dto);

        // Assert
        expected.Should().BeEmpty();
        _sequentialValidator.DidNotReceive().FindSequentialPhrase(
            Arg.Any<List<string>>(),
            Arg.Any<HashSet<string>>(),
            Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenNoDocumentsContainAllWords()
    {
        // Arrange
        var phrase = "hello world";
        _tokenizer.Tokenize(phrase).Returns(["HELLO", "WORLD"]);

        var dto = CreateModifiedIndexDto(dto =>
        {
            dto.InvertedIndexMap.Remove("WORLD");
        });

        // Act
        var expected = _sut.Search(phrase, dto);

        // Assert
        expected.Should().BeEmpty();
        _sequentialValidator.DidNotReceive().FindSequentialPhrase(
            Arg.Any<List<string>>(),
            Arg.Any<HashSet<string>>(),
            Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenInputIsEmpty()
    {
        // Arrange
        var input = "  ";
        var dto = CreateTestIndexDto();
        _tokenizer.Tokenize(input).Returns(new List<string>());

        // Act
        var result = _sut.Search(input, dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _tokenizer.DidNotReceive().Tokenize(Arg.Any<string>());
        _sequentialValidator.DidNotReceive().FindSequentialPhrase(
            Arg.Any<List<string>>(),
            Arg.Any<HashSet<string>>(),
            Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void Search_ShouldReturnEmptySet_WhenInputIsNull()
    {
        // Arrange
        string input = null;
        var dto = CreateTestIndexDto();

        // Act
        var result = _sut.Search(input, dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
        _tokenizer.DidNotReceive().Tokenize(Arg.Any<string>());
        _sequentialValidator.DidNotReceive().FindSequentialPhrase(
            Arg.Any<List<string>>(),
            Arg.Any<HashSet<string>>(),
            Arg.Any<InvertedIndexDto>());
    }

    [Fact]
    public void Search_ShouldReturnSingleWordDocuments_WhenInputHasOneWord()
    {
        // Arrange
        var input = "hello";
        _tokenizer.Tokenize(input).Returns(new[] { "HELLO" });

        var dto = CreateTestIndexDto();
        _sequentialValidator.FindSequentialPhrase(
            Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "HELLO" })),
            Arg.Is<HashSet<string>>(x => x.SetEquals(new[] { "doc1", "doc2", "doc3" })),
            Arg.Any<InvertedIndexDto>())
            .Returns(new HashSet<string>(new[] { "doc1", "doc2", "doc3" }));

        // Act
        var result = _sut.Search(input, dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new[] { "doc1", "doc2", "doc3" });
    }

    [Fact]
    public void Search_ShouldBeCaseInsensitive_WhenTokenizingInput()
    {
        // Arrange
        var input = "HeLLo WoRLD";
        _tokenizer.Tokenize(input).Returns(new[] { "HELLO", "WORLD" });

        var dto = CreateTestIndexDto();
        _sequentialValidator.FindSequentialPhrase(
            Arg.Is<List<string>>(x => x.SequenceEqual(new[] { "HELLO", "WORLD" })),
            Arg.Any<HashSet<string>>(),
            Arg.Any<InvertedIndexDto>())
            .Returns(new HashSet<string>(new[] { "doc1", "doc3" }));

        // Act
        var result = _sut.Search(input, dto);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(new[] { "doc1", "doc3" });
    }

    private static InvertedIndexDto CreateTestIndexDto()
    {
        return new InvertedIndexDto
        {
            AllDocuments = new HashSet<string>(["doc1", "doc2", "doc3"]),
            InvertedIndexMap = new SortedDictionary<string, HashSet<DocumentInfo>>
            {
                ["HELLO"] = new()
                {
                    new() { DocId = "doc1", Indexes = [10] },
                    new() { DocId = "doc2", Indexes = [5] },
                    new() { DocId = "doc3", Indexes = [15] }
                },
                ["WORLD"] = new()
                {
                    new() { DocId = "doc1", Indexes = [11] },
                    new() { DocId = "doc2", Indexes = [8] }, // Not sequential with HELLO@5
                    new() { DocId = "doc3", Indexes = [16] }
                }
            }
        };
    }

    private static InvertedIndexDto CreateModifiedIndexDto(Action<InvertedIndexDto> modifier)
    {
        var dto = CreateTestIndexDto();
        modifier(dto);
        return dto;
    }
}