using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed;

public class AuthorizeAsReddit : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var settings = context.HttpContext.RequestServices.GetRequiredService<AppSettings>();
        var requiredHeader = $"bearer {settings.Reddit.RefreshToken}";
        var requestAuthHeaderValue = context.HttpContext.Request.Headers.Authorization.FirstOrDefault();
        var actualHeader = "b" + (requestAuthHeaderValue?.Substring(1) ?? string.Empty);
        if (requestAuthHeaderValue == null || actualHeader != requiredHeader)
        {
            context.Result = new ForbidResult();
        }
    }
}
