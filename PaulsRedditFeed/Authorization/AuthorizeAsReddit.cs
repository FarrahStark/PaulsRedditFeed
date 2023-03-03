using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace PaulsRedditFeed;

public class AuthorizeAsReddit : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var settings = context.HttpContext.RequestServices.GetRequiredService<AppSettings>();
        var requiredHeader = $"Bearer {settings.Reddit.RefreshToken}";
        var actualHeader = context.HttpContext.Request.Headers.Authorization.FirstOrDefault() ?? string.Empty;
        if (actualHeader != requiredHeader)
        {
            context.Result = new ForbidResult();
        }
    }
}
