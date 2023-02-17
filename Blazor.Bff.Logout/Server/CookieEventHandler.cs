using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Blazor.Bff.Logout.Server;

public static class CookieEventHandler //: CookieAuthenticationEvents
{
    public static async Task SlidingExpirationAsync(CookieSlidingExpirationContext context)
    {
        if (context.Principal!.Identity!.IsAuthenticated)
        {
            if (context.ElapsedTime > TimeSpan.FromMinutes(3))
            {
                // TODO fix for js calls
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
        }
    }
}
