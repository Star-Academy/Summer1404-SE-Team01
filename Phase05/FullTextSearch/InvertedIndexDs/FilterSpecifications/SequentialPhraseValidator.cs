using FullTextSearch.InvertedIndexDs.Dtos;
using FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;

namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public class SequentialPhraseValidator : ISequentialPhrase
{
    public void ValidateSequentiality(SortedSet<string> docIds, IEnumerable<string> phrase, InvertedIndexDto dto)
    {
        var resultDocIds = new SortedSet<string>();
        foreach (var docId in docIds)
        {
            var firstWord = phrase.First();
            var firstWordDocInfo = dto.InvertedIndexMap[firstWord].FirstOrDefault(d => d.DocId == docId);
            var firstWordIndexes = firstWordDocInfo!.Indexes;
            
            var commonIndexesOfCurrentDocId = new SortedSet<long>(firstWordIndexes);
            
            bool isDocIdHasThisPhrase = true;
            for (int i = 1; i < phrase.Count(); i++)
            {
                var word = phrase.ElementAt(i);
                var wordDocInfo = dto.InvertedIndexMap[word].FirstOrDefault(d => d.DocId == docId);
                var wordIndexesReducedI =  new SortedSet<long>(
                    wordDocInfo!.Indexes.Select(index => index - i)
                );
                commonIndexesOfCurrentDocId.IntersectWith(wordIndexesReducedI);
                
                if (!commonIndexesOfCurrentDocId.Any()) // if no index is remained
                {
                    isDocIdHasThisPhrase = false;
                    break;
                }
            }

            if (isDocIdHasThisPhrase)
            {
                resultDocIds.Add(docId);
            }
        }
        
        docIds.IntersectWith(resultDocIds);
        
    }
}