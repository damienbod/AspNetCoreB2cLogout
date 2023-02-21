using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace RazorB2cLogout.Pages;

public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        var getClaim = User.Claims.FirstOrDefault(c => c.Type == "sessiontimeout");

        return Page();
      
    }
}