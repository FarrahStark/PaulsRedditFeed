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
        if (requestAuthHeaderValue == null || requestAuthHeaderValue != requiredHeader)
        {
            context.Result = new ForbidResult();
        }
    }
}
