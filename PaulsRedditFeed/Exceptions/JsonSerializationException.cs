namespace PaulsRedditFeed;

public class JsonSerializationException : Exception
{
    public JsonSerializationException(string? message, Exception? innerException = null) : base(message, innerException)
    {
    }
}

