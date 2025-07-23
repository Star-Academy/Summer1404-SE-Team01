using System.Text.RegularExpressions;

namespace FullTextSearch.InvertedIndex.QueryBuilder
{
    public class QueryExtractor : IQueryExtractor
    {
        public List<string> ExtractQueries(string query, string pattern)
        {
            var words = Regex.Split(query!, @"\s+");

            var keyWords = words.Where(w => Regex.IsMatch(w, pattern)).Select(w => w.ToUpper()).ToList();

            return keyWords;
        }
    }
}
