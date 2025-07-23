
using NSubstitute;

using FluentAssertions;
using FullTextSearch.Services.FileReaderService;
using FullTextSearch.Services.LoggerService;
using NSubstitute.ExceptionExtensions;

namespace FullTextSearch.Tests;

public class FileReaderTests
{
    private readonly string _testDir;

    public FileReaderTests()
    {
        
        _testDir = "test_files";
        Directory.CreateDirectory(_testDir);

        File.WriteAllText(Path.Combine(_testDir, "A.txt"), "a");
        File.WriteAllText(Path.Combine(_testDir, "B.txt"), "b");
        File.WriteAllText(Path.Combine(_testDir, "C.txt"), "c");
    }

    [Fact]
    public void ReadAllFiles_ShouldReturnCorrectContents()
    {
        var reader = new FileReader();

        var result = reader.ReadAllFiles(_testDir);

        result.Should().HaveCount(3);
        result.Should().ContainKey("A.txt").WhoseValue.Should().Be("a");
        result.Should().ContainKey("B.txt").WhoseValue.Should().Be("b");
        result.Should().ContainKey("C.txt").WhoseValue.Should().Be("c");
    }

    [Fact]
    public void ReadAllFiles_ThrowException_WhenDirectoryDoesNotExist()
    {
        var reader = new FileReader();
        var nonExistentPath = Path.Combine(_testDir, "not_found");
        
        reader.Invoking(r => r.ReadAllFiles(nonExistentPath))
            .Should().Throw<DirectoryNotFoundException>()
            .WithMessage($"Directory not found: {nonExistentPath}");
    }

    [Fact]
    public void ReadAllFiles_ShouldThrowException_WhenDirectoryIsEmpty()
    {
        var reader = new FileReader();
        
        var emptyDirectory = Path.Combine(_testDir, "fake.txt");

        var result = reader.ReadAllFiles(emptyDirectory);

        result.Should().BeEmpty();
    }
}
