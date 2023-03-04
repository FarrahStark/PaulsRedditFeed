namespace PaulsRedditFeed;

/// <summary>
/// An Exception that indicates something when wrong trying to deserialize something to Json
/// </summary>
public class JsonSerializationException : Exception
{
    public JsonSerializationException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }
}

