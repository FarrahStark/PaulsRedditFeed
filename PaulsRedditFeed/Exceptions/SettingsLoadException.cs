namespace PaulsRedditFeed;

/// <summary>
/// An exception that indicates settings weren't loaded correctly
/// </summary>
public class SettingsLoadException : Exception
{
    public SettingsLoadException(string message) : base(message)
    {
    }

    public SettingsLoadException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
