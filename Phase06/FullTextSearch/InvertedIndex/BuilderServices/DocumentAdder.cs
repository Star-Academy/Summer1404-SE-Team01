using FullTextSearch.InvertedIndex.BuilderServices.Abstractions;
using FullTextSearch.InvertedIndex.Dtos;
using FullTextSearch.Services.TokenizerService;

namespace FullTextSearch.InvertedIndex.BuilderServices;

public class DocumentAdder : IDocumentAdder
{
    private readonly ITokenizer _tokenizer;

    public DocumentAdder(ITokenizer tokenizer)
    {
        _tokenizer = tokenizer ?? throw new ArgumentNullException(nameof(tokenizer));
    }

    public void AddDocument(string docId, string contents, InvertedIndexDto invIdxDto)
    {
        var tokens = _tokenizer.Tokenize(contents);

        foreach (var (word, index) in tokens.Select((w, i) => (w, i)))
        {
            if (!invIdxDto.InvertedIndexMap.ContainsKey(word))
            {
                invIdxDto.InvertedIndexMap[word] = new();
            }

            DocumentInfo documentInfo;
            if (invIdxDto.InvertedIndexMap[word].All(docInfo => docInfo.DocId != docId))
            {
                documentInfo = new DocumentInfo
                {
                    DocId = docId,
                    Indexes = new()
                };
                invIdxDto.InvertedIndexMap[word].Add(documentInfo);
            }
            else
            {
                documentInfo = invIdxDto.InvertedIndexMap[word].Single(docInfo => docInfo.DocId == docId);
            }

            documentInfo.Indexes.Add(index);
        }


    }
}