namespace FullTextSearch.InvertedIndex.Dtos;

public class InvertedIndexDto
{
    public required SortedDictionary<string, SortedSet<DocumentInfo>> InvertedIndexMap { get; set; }
    public required SortedSet<string> AllDocuments { get; set; }
}