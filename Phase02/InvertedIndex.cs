using System.Text.RegularExpressions;

namespace FullTextSearch
{
    public static class InvertedIndex
    {
        public static InvertedIndexDto Build(Dictionary<string, string> docs)
        {
            var dto = new InvertedIndexDto
            {
                InvertedIndexMap = new(),
                DocIds = new()
            };

            foreach (var (docId, content) in docs)
            {
                var words = Regex.Split(content, @"[^\w']+")
                                 .Where(w => !string.IsNullOrWhiteSpace(w))
                                 .Select(w => w.ToUpper());

                foreach (var word in words)
                {
                    if (!dto.InvertedIndexMap.TryGetValue(word, out var postings))
                    {
                        postings = new SortedSet<string>();
                        dto.InvertedIndexMap[word] = postings;
                    }
                    postings.Add(docId);
                }
            }

            dto.DocIds = new SortedSet<string>(docs.Keys);
            return dto;
        }

        public static SortedSet<string> SearchWord(string word, InvertedIndexDto dto)
        {
            if (string.IsNullOrWhiteSpace(word))
                throw new ArgumentNullException(nameof(word));

            var key = word.Trim().ToUpper();
            return dto.InvertedIndexMap.TryGetValue(key, out var result)
                ? new SortedSet<string>(result)
                : new SortedSet<string>();
        }

        public static SortedSet<string> AdvancedSearch(string input, InvertedIndexDto dto)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input));

            var tokens = Regex.Split(input.Trim(), @"\s+");
            var must = tokens.Where(t => !t.StartsWith("+") && !t.StartsWith("-")).ToList();
            var optional = tokens.Where(t => t.StartsWith("+")).Select(t => t.Substring(1)).ToList();
            var excluded = tokens.Where(t => t.StartsWith("-")).Select(t => t.Substring(1)).ToList();

            var result = new SortedSet<string>(dto.DocIds);

            foreach (var term in must)
                result.IntersectWith(SearchWord(term, dto));

            if (optional.Any())
            {
                var optSet = new SortedSet<string>();
                foreach (var term in optional)
                    optSet.UnionWith(SearchWord(term, dto));
                result.IntersectWith(optSet);
            }

            foreach (var term in excluded)
                result.ExceptWith(SearchWord(term, dto));

            return result;
        }
    }
}