namespace FullTextSearch.Exceptions;

public class EmptyDirectoryException : Exception
{
    public EmptyDirectoryException(string message) : base(message)
    {
    }
}
