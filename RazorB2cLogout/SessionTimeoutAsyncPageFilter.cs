using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;

namespace RazorB2cLogout;

public class SessionTimeoutAsyncPageFilter : IAsyncPageFilter
{
    private readonly IDistributedCache _cache;
    private readonly int _timeoutInMinutes = 3;
    private const int _cacheExpirationInDays = 2;

    public SessionTimeoutAsyncPageFilter(IDistributedCache cache)
    {
        _cache = cache;
    }

    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        if (context.HttpContext.Request.Path.HasValue 
            && !context.HttpContext.Request.Path.Value.Contains( "/SignedOut"))
        {
            var claimTypes = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
            var name = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == claimTypes)!.Value;

            if (name == null) throw new ArgumentNullException(nameof(name));

            var lastActivity = GetFromCache(name);

            if (lastActivity != null && lastActivity.GetValueOrDefault().AddMinutes(_timeoutInMinutes) < DateTime.UtcNow)
            {
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }

            AddUpdateCache(name);
        }

        await next.Invoke();
    }

    private void AddUpdateCache(string name)
    {
        var options = new DistributedCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromDays(_cacheExpirationInDays));

        _cache.SetString(name, DateTime.UtcNow.ToString("s"), options);
    }

    private DateTime? GetFromCache(string key)
    {
        var item = _cache.GetString(key);
        if (item != null)
        {
            return DateTime.Parse(item);
        }

        return null;
    }
}