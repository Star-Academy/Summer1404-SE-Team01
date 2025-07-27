
namespace FullTextSearch.InvertedIndexDs.FilterSpecifications;

public class OptionalPhraseSpecification : IPhraseSpecification
{
    public Dictionary<string, string> Documents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public List<string> Keywords => throw new NotImplementedException();

    public void FilterDocumentsByQuery(SortedSet<string> result)
    {
        throw new NotImplementedException();
    }
}
