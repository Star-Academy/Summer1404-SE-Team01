namespace FullTextSearch.InvertedIndex.QueryBuilder
{
    public class SearchQuery
    {
        public List<string> NecessaryWords { get; set; }
        public List<string> OptionalWords { get; set; }
        public List<string> ExcludedWords { get; set; }
    }
}
