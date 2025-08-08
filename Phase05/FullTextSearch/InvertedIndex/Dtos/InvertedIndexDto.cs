namespace FullTextSearch.InvertedIndex.Dtos;

public class InvertedIndexDto
{
    public required SortedDictionary<string, HashSet<DocumentInfo>> InvertedIndexMap { get; init; }
    public required HashSet<string> AllDocuments { get; init; }
}