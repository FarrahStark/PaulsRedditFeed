namespace PaulsRedditFeed
{
    public class CachingException : Exception
    {
        public CachingException(string message, Exception? innerException = null) : base(message, innerException)
        {
        }
    }
}
