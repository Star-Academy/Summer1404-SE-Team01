using System.Text.RegularExpressions;

namespace FullTextSearch;

class InvertedIndex
{
    private static SortedDictionary<string, SortedSet<string>> _invertedIndexMap = new SortedDictionary<string, SortedSet<string>>();
    private static SortedSet<string> _docIds;

    public static void BuildIndexMap(Dictionary<string, string> docs)
    {
        foreach ((var docId, var content) in docs)
        {
            var words = Regex.Split(content, @"[^\w']+");

            _invertedIndexMap = words.
                Where(word => !string.IsNullOrWhiteSpace(word))
                .Select(word => word.ToUpper())
                .Aggregate(_invertedIndexMap, (indexMap, word) =>
                {
                    if (!indexMap.ContainsKey(word))
                    {
                        indexMap[word] = new SortedSet<string>();
                    }
                    indexMap[word].Add(docId);
                    return indexMap;
                });
        }

        _docIds = new SortedSet<string>(docs.Keys);
    }

    public static SortedSet<string> SearchWord(string word)
    {
        var upperWord = word.ToUpper();
        _invertedIndexMap.TryGetValue(upperWord, out SortedSet<string>? result);
        return result ?? new SortedSet<string>();
    }

    public static SortedSet<string> AdvancedSearch(string input)
    {

        var words = Regex.Split(input!, @"\s+");

        var necessaryWords = words.Where(w => Regex.IsMatch(w, @"^[^-+]\w+")).ToList();
        var optionalWords = words.Where(w => Regex.IsMatch(w, @"^\+\w+")).ToList();
        var excludedWords = words.Where(w => Regex.IsMatch(w, @"^\-\w+")).ToList();


        var result = new SortedSet<string>(_docIds);

        foreach (var word in necessaryWords)
        {
            var currentDocIds = SearchWord(word);

            result.IntersectWith(currentDocIds);
        }

        var optionalDocIds = new SortedSet<string>();
        foreach (var word in optionalWords)
        {
            var currentDocIds = SearchWord(word);
            optionalDocIds.UnionWith(currentDocIds);
        }

        if (optionalDocIds.Count != 0)
        {
            result.IntersectWith(optionalDocIds);
        }

        foreach (var word in excludedWords)
        {
            var currentDocIds = SearchWord(word);
            result.ExceptWith(currentDocIds);
        }

        return result;
    }

}