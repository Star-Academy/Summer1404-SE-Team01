namespace FullTextSearch.InvertedIndex.Dtos;

public class DocumentInfo
{

    public required string DocId { get; init; }
    public required SortedSet<long> Indexes { get; set; }
}