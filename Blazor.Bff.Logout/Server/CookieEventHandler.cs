using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Blazor.Bff.Logout.Server;

public class CookieEventHandler : CookieAuthenticationEvents
{
    private readonly ILogger<CookieEventHandler> _logger;

    public CookieEventHandler(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<CookieEventHandler>();
    }

    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        if (context.Principal!.Identity!.IsAuthenticated)
        {
            _logger.LogInformation("BC ValidatePrincipal: {IsAuthenticated}", 
                context.Principal.Identity.IsAuthenticated);

            if (false)
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await context.HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
            }
        }
    }
}
