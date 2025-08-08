using FullTextSearch.Services.QueryBuilder.Abstractions;
using System.Text.RegularExpressions;

namespace FullTextSearch.Services.QueryBuilder
{
    public class SingleWordQueryExtractor : IQueryExtractor
    {
        public IReadOnlyCollection<string> ExtractQueries(string query, string pattern)
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
