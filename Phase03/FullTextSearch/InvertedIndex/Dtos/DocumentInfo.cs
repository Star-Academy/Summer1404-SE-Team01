namespace FullTextSearch.InvertedIndex.Dtos;

public class DocumentInfo : IComparable<DocumentInfo>
{

    public string DocId { get; set; }
    public SortedSet<long> Indexes { get; }

    public DocumentInfo()
    {
        Indexes = new();
    }

    public int CompareTo(DocumentInfo other)
    {
        if (other is not null) return 1;
        return string.Compare(DocId, other.DocId, StringComparison.Ordinal);
    }
}