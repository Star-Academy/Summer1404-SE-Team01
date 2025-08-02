using FluentAssertions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.InvertedIndex.SearchFeatures;
using FullTextSearch.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.Tests.SearchFeaturesTests;

public class SequentialValidatorTests
{
    private readonly ISequentialValidator _sut;

    public SequentialValidatorTests()
    {
        _sut = new SequentialValidator();
    }

    [Fact]
    public void Validate_ShouldReturnDocumentsWithExactPhrase_WhenWordsAppearSequentially()
    {
        // Arrange
        var words = new List<string> { "hello", "world", "phrase" };
        var candidateDocs = new SortedSet<string> { "doc1", "doc3" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1"]);
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenNoDocumentsContainFullSequence()
    {
        // Arrange
        var words = new List<string> { "hello", "world", "missing" };
        var candidateDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnAllDocuments_WhenAllContainExactTwoWordSequence()
    {
        // Arrange
        var words = new List<string> { "hello", "world" };
        var candidateDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1", "doc2", "doc3"]);
    }

    [Fact]
    public void Validate_ShouldHandleMultipleSequencesInSameDocument()
    {
        // Arrange
        var words = new List<string> { "hello", "world" };
        var candidateDocs = new SortedSet<string> { "doc1" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1"]);
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenCandidateDocsIsEmpty()
    {
        // Arrange
        var words = new List<string> { "hello", "world" };
        var candidateDocs = new SortedSet<string>();
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenWordsListIsEmpty()
    {
        // Arrange
        var words = new List<string>();
        var candidateDocs = new SortedSet<string> { "doc1", "doc2" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnAllDocuments_WhenPhraseIsSingleWord()
    {
        // Arrange
        var words = new List<string> { "hello" };
        var candidateDocs = new SortedSet<string> { "doc1", "doc2", "doc3" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1", "doc2", "doc3"]);
    }

    [Fact]
    public void Validate_ShouldIgnoreNonSequentialMatches()
    {
        // Arrange
        var invertedIndexDto = CreateModifiedIndexDto(dto =>
        {
            dto.InvertedIndexMap["world"].Add(
                new DocumentInfo { DocId = "doc1", Indexes = [30] });
        });

        var words = new List<string> { "hello", "world" };
        var candidateDocs = new SortedSet<string> { "doc1" };

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEquivalentTo(["doc1"]);
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenWordPositionsDontMatchSequence()
    {
        // Arrange
        var invertedIndexDto = CreateModifiedIndexDto(dto =>
        {
            dto.InvertedIndexMap["world"].Single(d => d.DocId == "doc2").Indexes = [10]; // Doesn't follow hello@5
        });

        var words = new List<string> { "hello", "world" };
        var candidateDocs = new SortedSet<string> { "doc2" };

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenFirstWordMissingFromIndex()
    {
        // Arrange
        var words = new List<string> { "missing", "world" };
        var candidateDocs = new SortedSet<string> { "doc1", "doc2" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenSubsequentWordMissingFromIndex()
    {
        // Arrange
        var words = new List<string> { "hello", "missing" };
        var candidateDocs = new SortedSet<string> { "doc1", "doc2" };
        var invertedIndexDto = CreateTestInvertedIndexDto();

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenDocumentMissingFirstWord()
    {
        // Arrange
        var invertedIndexDto = CreateModifiedIndexDto(dto =>
        {
            dto.InvertedIndexMap["world"].RemoveWhere(d => d.DocId == "doc1");
        });

        var words = new List<string> { "world", "phrase" };
        var candidateDocs = new SortedSet<string> { "doc2" };

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldReturnEmptySet_WhenDocumentMissingSubsequentWord()
    {
        // Arrange
        var invertedIndexDto = CreateTestInvertedIndexDto();

        var words = new List<string> { "hello", "phrase" };
        var candidateDocs = new SortedSet<string> { "doc1" };

        // Act
        var expected = _sut.Validate(words, candidateDocs, invertedIndexDto);

        // Assert
        expected.Should().BeEmpty();
    }

    private static InvertedIndexDto CreateTestInvertedIndexDto()
    {
        return new InvertedIndexDto
        {
            AllDocuments = new SortedSet<string> { "doc1", "doc2", "doc3" },
            InvertedIndexMap = new SortedDictionary<string, SortedSet<DocumentInfo>>
            {
                ["hello"] = new()
                {
                    new() { DocId = "doc1", Indexes = [10, 20] },
                    new() { DocId = "doc2", Indexes = [5] },
                    new() { DocId = "doc3", Indexes = [15] }
                },
                ["world"] = new()
                {
                    new() { DocId = "doc1", Indexes = [11, 21] },
                    new() { DocId = "doc2", Indexes = [6] },
                    new() { DocId = "doc3", Indexes = [16] }
                },
                ["phrase"] = new()
                {
                    new() { DocId = "doc1", Indexes = [12, 22] },
                    new() { DocId = "doc3", Indexes = [20] }
                }
            }
        };
    }

    private static InvertedIndexDto CreateModifiedIndexDto(Action<InvertedIndexDto> modifier)
    {
        var dto = CreateTestInvertedIndexDto();
        modifier(dto);
        return dto;
    }
}