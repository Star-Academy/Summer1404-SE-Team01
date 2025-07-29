using FullTextSearch.InvertedIndex.QueryBuilder.Abstractions;
using System.Text.RegularExpressions;

namespace FullTextSearch.InvertedIndex.QueryBuilder
{
    public class SingleWordQueryExtractor : IQueryExtractor
    {
        public List<string> ExtractQueries(string query, string pattern)
        {
            string queryWithoutPhrase = Regex.Replace(query!, @"[-+]?""[^""]+""", "").Trim();
            var words = Regex.Split(queryWithoutPhrase, @"\s+");

            var keyWords = words
                 .Where(w => Regex.IsMatch(w, pattern))
                 .Select(w => Regex.Replace(w, @"^[+-]", string.Empty).ToUpper())
                 .ToList();

            return keyWords;
        }
    }
}
