namespace FullTextSearch
{
    public class InvertedIndexDto
    {
        public required SortedDictionary<string, HashSet<string>> InvertedIndexMap { get; set; }
        public required HashSet<string> DocIds { get; set; }
    }
}