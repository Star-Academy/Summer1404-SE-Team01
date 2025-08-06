namespace FullTextSearch.API.InvertedIndex.Dtos;

public class InvertedIndexDto
{
    public required SortedDictionary<string, SortedSet<DocumentInfo>> InvertedIndexMap { get; set; }
    public required HashSet<string> AllDocuments { get; set; }
}