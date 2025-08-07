using System.Diagnostics.CodeAnalysis;

namespace FullTextSearch.API.InvertedIndex.Dtos;

[ExcludeFromCodeCoverage]
public class DocumentInfo
{

    public required string DocId { get; set; }
    public required SortedSet<long> Indexes { get; set; }
}