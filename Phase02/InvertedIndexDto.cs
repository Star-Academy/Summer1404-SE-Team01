namespace FullTextSearch
{
    public class InvertedIndexDto
    {
        public required SortedDictionary<string, SortedSet<string>> InvertedIndexMap { get; set; }
        public required SortedSet<string> DocIds { get; set; }
    }
}