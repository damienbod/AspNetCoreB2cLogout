using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace RazorB2cLogout;

public class SessionTimeoutAsyncPageFilter : IAsyncPageFilter
{
    public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
    {
        return Task.CompletedTask;
    }

    public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
    {
        var getClaim = context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sessiontimeout");

        if (getClaim != null)
        {
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await context.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }

        await next.Invoke();
    }
}
