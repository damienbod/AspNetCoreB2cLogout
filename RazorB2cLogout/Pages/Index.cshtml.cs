using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorB2cLogout.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var claims = User.Claims.ToList();

        return Page();
    }
}