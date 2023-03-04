namespace PaulsRedditFeed;

/// <summary>
/// Model used by ASP.NET to display develper error page information
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}