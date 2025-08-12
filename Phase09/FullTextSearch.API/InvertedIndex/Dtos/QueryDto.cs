namespace FullTextSearch.API.InvertedIndex.Dtos;

public class QueryDto
{

    public List<string> Required { get; set; } = new();
    public List<string> Optional { get; set; } = new();
    public List<string> Excluded { get; set; } = new();

}