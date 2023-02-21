using System;

namespace PaulsRedditFeed
{
    public class SettingsLoadException : Exception
    {
        public SettingsLoadException(string message) : base(message)
        {
        }

        public SettingsLoadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
