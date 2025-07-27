using FluentAssertions;
using FullTextSearch.Exceptions;
using FullTextSearch.Services.FileReaderService;

namespace FullTextSearch.Tests;

public class FileReaderTests
{
    private readonly string _testDir;
    private readonly FileReader _reader;

    public FileReaderTests()
    {
        _testDir = "testDir";
        Directory.CreateDirectory(_testDir);
        _reader = new FileReader();
    }

    [Fact]
    public void ReadAllFiles_ShouldReturnCorrectContents()
    {
        File.WriteAllText(Path.Combine(_testDir, "A.txt"), "a");
        File.WriteAllText(Path.Combine(_testDir, "B.txt"), "b");
        File.WriteAllText(Path.Combine(_testDir, "C.txt"), "c");
        var result = _reader.ReadAllFiles(_testDir);

        result.Should().HaveCount(3);
        result.Should().ContainKey("A.txt").WhoseValue.Should().Be("a");
        result.Should().ContainKey("B.txt").WhoseValue.Should().Be("b");
        result.Should().ContainKey("C.txt").WhoseValue.Should().Be("c");
    }

    [Fact]
    public void ReadAllFiles_ThrowException_WhenDirectoryDoesNotExist()
    {
        var nonExistentPath = Path.Combine(_testDir, "not_found");

        Action act = () => _reader.ReadAllFiles(nonExistentPath);

        act.Should().Throw<DirectoryNotFoundException>().WithMessage($"Directory {nonExistentPath} not found!");
    }

    [Fact]
    public void ReadAllFiles_ShouldThrowException_WhenDirectoryIsEmpty()
    {
        var emptyDir = "empty";
        Directory.CreateDirectory(emptyDir);

        Action act = () => _reader.ReadAllFiles(emptyDir);

        act.Should().Throw<EmptyDirectoryException>().WithMessage($"Directory {emptyDir} is empty.");
    }
}
