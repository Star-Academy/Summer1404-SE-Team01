namespace FullTextSearch.InvertedIndex.Dtos;

public class DocumentInfo : IComparable<DocumentInfo>
{

    public required string DocId { get; set; }
    public required SortedSet<long> Indexes { get; set; }
    public int CompareTo(DocumentInfo other)
    {
        if (other is not null) return 1;
        return string.Compare(DocId, other.DocId, StringComparison.Ordinal);
    }
}