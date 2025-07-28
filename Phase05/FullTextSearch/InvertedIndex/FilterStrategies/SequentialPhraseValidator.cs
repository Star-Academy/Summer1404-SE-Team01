//using FullTextSearch.InvertedIndexDs.Dtos;
//using FullTextSearch.InvertedIndexDs.FilterSpecifications.Abstractions;

//namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

//public class SequentialPhraseValidator : ISequentialPhrase
//{
//    public void FindConsecutivePhrases(SortedSet<string> docIds, IEnumerable<string> phrase, InvertedIndexDto dto)
//    {
//        var resultDocIds = new SortedSet<string>();
//        foreach (var word in phrase)
//        {
//            if (!dto.InvertedIndexMap.TryGetValue(word, out var wordDocIds))
//            {
//                continue;
//            }
//            // Find documents that contain the current word
//            var currentDocIds = new SortedSet<string>(wordDocIds);

//            // If this is the first word, initialize resultDocIds
//            if (resultDocIds.Count == 0)
//            {
//                resultDocIds.UnionWith(currentDocIds);
//            }
//            else
//            {
//                // Intersect with the current word's document IDs
//                resultDocIds.IntersectWith(currentDocIds);
//            }
//        }

//        docIds.IntersectWith(resultDocIds);

//    }
//}