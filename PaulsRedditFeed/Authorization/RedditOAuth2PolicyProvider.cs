//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;

//namespace PaulsRedditFeed.Authorization
//{
//    public class RedditOAuth2PolicyProvider : IAuthorizationPolicyProvider
//    {
//        public const string PolicyName = "reddit Oauth2";
//        private readonly AppSettings settings;

//        public RedditOAuth2PolicyProvider(AppSettings settings)
//        {
//            this.settings = settings;
//        }

//        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
//            Task.FromResult(new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());

//        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
//        {
//            return Task.FromResult((AuthorizationPolicy?)null);
//        }

//        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
//        {
//            if (policyName == PolicyName)
//            {
//                var policy = new AuthorizationPolicyBuilder();
//                policy.AddRequirements(new RedditOAuth2Requirement(settings.Reddit.RefreshToken));
//                return Task.FromResult(policy.Build());
//            }

//            return Task.FromResult<AuthorizationPolicy>(null);
//        }
//    }
//}
