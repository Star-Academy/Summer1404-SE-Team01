using System.Text.RegularExpressions;

namespace FullTextSearch.InvertedIndex.QueryBuilder
{
    public class QueryExtractor : IQueryExtractor
    {
        public SearchQuery ExtractQueries(string query)
        {
            var words = Regex.Split(query!, @"\s+");

            var necessaryWords = words.Where(w => Regex.IsMatch(w, @"^[^-+]\w+")).ToList();
            var optionalWords = words.Where(w => Regex.IsMatch(w, @"^\+\w+")).ToList();
            var excludedWords = words.Where(w => Regex.IsMatch(w, @"^\-\w+")).ToList();

            return new SearchQuery
            {
                NecessaryWords = necessaryWords,
                OptionalWords = optionalWords,
                ExcludedWords = excludedWords
            };
        }
    }
}
