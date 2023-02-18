using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blazor.Bff.Logout.Server.Controllers;

[ValidateAntiForgeryToken]
[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class DirectApiController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var getClaim = User.Claims.FirstOrDefault(c => c.Type== "sessiontimeout");

        if (getClaim != null) return Unauthorized("sessiontimeout");

        return Ok(new List<string> { "some data", "more data", "loads of data" });
    }
}
