using FullTextSearch.API.InvertedIndex.Dtos;
using FullTextSearch.API.InvertedIndex.SearchFeatures.Abstractions;

namespace FullTextSearch.API.InvertedIndex.SearchFeatures;

public class SequentialPhraseFinder : ISequentialPhraseFinder
{
    public HashSet<string> FindSequentialPhrase(IList<string> words, HashSet<string> docIdsContainingWords, InvertedIndexDto invIdxDto)
    {
        var result = new HashSet<string>();

        if (words.Count == 0 || docIdsContainingWords.Count == 0)
        {
            return result;
        }

        var firstWord = words[0];
        if (!invIdxDto.InvertedIndexMap.TryGetValue(firstWord, out var firstWordDocInfos))
        {
            return result;
        }

        foreach (var docId in docIdsContainingWords)
        {
            var firstWordDocInfo = firstWordDocInfos.FirstOrDefault(d => d.DocId == docId);

            var firstWordCurrentDocIdIndexes = firstWordDocInfo.Indexes;
            var commonIndexesOfCurrentDocId = new SortedSet<long>(firstWordCurrentDocIdIndexes);

            bool isDocIdHasThisPhrase = true;

            for (int i = 1; i < words.Count; i++)
            {
                var word = words[i];
                if (!invIdxDto.InvertedIndexMap.TryGetValue(word, out var wordDocInfos))
                {
                    isDocIdHasThisPhrase = false;
                    break;
                }
                var wordDocInfo = wordDocInfos.FirstOrDefault(d => d.DocId == docId);
                if (wordDocInfo == null)
                {
                    isDocIdHasThisPhrase = false;
                    break;
                }

                var wordIndexesReducedI = new SortedSet<long>(
                    wordDocInfo.Indexes.Select(index => index - i)
                );
                commonIndexesOfCurrentDocId.IntersectWith(wordIndexesReducedI);

                if (!commonIndexesOfCurrentDocId.Any())
                {
                    isDocIdHasThisPhrase = false;
                    break;
                }
            }

            if (isDocIdHasThisPhrase)
            {
                result.Add(docId);
            }
        }

        return result;
    }
}