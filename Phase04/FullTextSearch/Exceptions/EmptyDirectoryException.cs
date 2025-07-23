namespace FullTextSearch.Exceptions;

public class EmptyDirectoryException : Exception
{
    public EmptyDirectoryException(string dir) : base($"Directory {dir} is empty.")
    {
    }
}