namespace FullTextSearch.InvertedIndex.Dtos;

public class DocumentInfo
{

    public required string DocId { get; set; }
    public required SortedSet<long> Indexes { get; set; }
}