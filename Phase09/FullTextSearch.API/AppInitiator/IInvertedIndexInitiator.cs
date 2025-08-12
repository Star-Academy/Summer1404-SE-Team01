using FullTextSearch.API.InvertedIndex.Dtos;

namespace FullTextSearch.API.AppInitiator;

public interface IInvertedIndexInitiator
{
    InvertedIndexDto GetData();
}