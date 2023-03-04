namespace PaulsRedditFeed;

/// <summary>
/// An exception that indicates a problem with interfacing with the cache
/// </summary>
public class CachingException : Exception
{
    public CachingException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
