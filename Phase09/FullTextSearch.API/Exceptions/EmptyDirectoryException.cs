using System.Diagnostics.CodeAnalysis;

namespace FullTextSearch.API.Exceptions;

[ExcludeFromCodeCoverage]
public class EmptyDirectoryException : Exception
{
    public EmptyDirectoryException()
    {

    }
    public EmptyDirectoryException(string message) : base(message)
    {

    }

    public EmptyDirectoryException(string message, Exception inner) : base(message, inner)
    {
    }


}