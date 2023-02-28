//using Microsoft.AspNetCore.Authorization;

//namespace PaulsRedditFeed.Authorization;
//public class RedditOAuth2Handler : AuthorizationHandler<RedditOAuth2Requirement>
//{
//    public const string PolicyName = "reddit Oauth2";
//    private readonly IHttpContextAccessor accessor;
//    private readonly AppSettings settings;

//    public RedditOAuth2Handler(IHttpContextAccessor accessor, AppSettings settings)
//    {
//        this.accessor = accessor;
//        this.settings = settings;
//    }

//    protected override Task HandleRequirementAsync(
//        AuthorizationHandlerContext context,
//        RedditOAuth2Requirement requirement)
//    {
//        var requiredHeader = $"bearer {settings.Reddit.RefreshToken}";
//        var requestAuthHeaderValue = accessor?.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
//        if (requestAuthHeaderValue != null && requestAuthHeaderValue == requiredHeader)
//        {
//            context.Succeed(requirement);
//        }
//        else
//        {
//            context.Fail();
//        }

//        return Task.CompletedTask;
//    }
//}
