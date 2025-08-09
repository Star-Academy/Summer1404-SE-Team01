using System.Diagnostics.CodeAnalysis;
using FullTextSearch.API.InvertedIndex.BuilderServices.Abstraction;
using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.Services.FileReaderService;

namespace FullTextSearch.API.AppInitiator;

[ExcludeFromCodeCoverage]
public class InvertedIndexInitiator :  IInvertedIndexInitiator
{
    private readonly IInvertedIndexBuilder _builder;
    private readonly IFileReader _fileReader;
    private readonly InvertedIndexDto _invIdxDto;

    public InvertedIndexInitiator(IFileReader fileReader, IInvertedIndexBuilder builder)
    {
        _fileReader = fileReader;
        _builder = builder;
        var documents = _fileReader.ReadAllFiles("EnglishData");
        _invIdxDto = _builder.Build(documents);
    }

    public InvertedIndexDto GetData() =>  _invIdxDto;
}