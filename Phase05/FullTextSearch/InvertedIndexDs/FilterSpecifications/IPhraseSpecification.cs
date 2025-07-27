namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public interface IPhraseSpecification : ISpecification
{
    Dictionary<string, string> Documents { get; set; }
}

