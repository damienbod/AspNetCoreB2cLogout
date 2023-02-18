using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Security.Claims;

namespace Blazor.Bff.Logout.Server;

public static class CookieEventHandler //: CookieAuthenticationEvents
{
    public static async Task SlidingExpirationAsync(CookieSlidingExpirationContext context)
    {
        if (context.Principal!.Identity!.IsAuthenticated)
        {
            if (context.ElapsedTime > TimeSpan.FromMinutes(3))
            {
                if (!context.Principal.HasClaim(c => c.Type == "timeout"))
                {
                    var claim = new Claim("sessiontimeout", DateTime.UtcNow.ToLongTimeString());
                    var claimsIdentity = new ClaimsIdentity(new[] { claim });
                    context.Principal.AddIdentity(claimsIdentity);
                }
            }
        }
    }
}
