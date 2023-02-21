using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace RazorB2cLogout.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var getClaim = User.Claims.FirstOrDefault(c => c.Type == "sessiontimeout");

        if (getClaim != null)
        {
            return SignOut(
                new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

        var claims = User.Claims.ToList();

        return Page();
      
    }
}