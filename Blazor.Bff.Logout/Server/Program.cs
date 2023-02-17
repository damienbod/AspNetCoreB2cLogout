using Blazor.Bff.Logout.Server;
using Blazor.Bff.Logout.Server.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.AddServerHeader = false;
});

var services = builder.Services;
var configuration = builder.Configuration;
var env = builder.Environment;
IServiceProvider? applicationServices = null;

services.AddScoped<CaeClaimsChallengeService>();

services.AddAntiforgery(options =>
{
    options.HeaderName = "X-XSRF-TOKEN";
    options.Cookie.Name = "__Host-X-XSRF-TOKEN";
    options.Cookie.SameSite = SameSiteMode.Strict;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

services.AddHttpClient();
services.AddOptions();

//services.AddMicrosoftIdentityWebAppAuthentication(configuration, "AzureB2C")
//    .EnableTokenAcquisitionToCallDownstreamApi(Array.Empty<string>())
//    .AddInMemoryTokenCaches();

builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(authOptions =>
    {
        configuration.Bind("AzureB2C", authOptions);
    }
    , sessionOptions =>
    {
        // This appears to be obsolete. Don't use it because you get the following error | OptionsValidationException: Cookie.Expiration is ignored, use ExpireTimeSpan instead.
        //sessionOptions.Cookie.Expiration = TimeSpan.FromMinutes(-1);

        // Setting the MaxAge will convert Expiration Date on the cookie from 'Session' to an actual future date and will show the default expiration date that is 14 days in the
        // future. However, note that the session cookie is different from the Authentication cookie, who's MaxAge is set in the AddMicrosoftIdentityWebApp extension method above 
        // with an expiration that is equal to the session cookie's expiration. Additionally, we set the SlidingExpiration to false. These three configurations are considered the
        // safest and most secure. Otherwise, not setting the MaxAge while leaving the SlidingExpiration false results in CORS policy errors on the console after the session
        // cookie has expired. 
        // Reference: https://brokul.dev/authentication-cookie-lifetime-and-sliding-expiration
        sessionOptions.Cookie.MaxAge = TimeSpan.FromMinutes(5);
        sessionOptions.Cookie.Name = "mycookie.Auth";
        sessionOptions.ExpireTimeSpan = TimeSpan.FromHours(2);
        sessionOptions.Events.OnValidatePrincipal = async e =>
        {
            var claims = e.HttpContext.User.Claims.ToList();

        };

        // Keep this false and never change it to true. The problem with SlidingExpiration enabled is that the authentication cookie could be potentially re-issued infinitely.
        // That's not a good security practice. If a hacker took control of the account, they could use it forever.
        sessionOptions.SlidingExpiration = false;
    }).EnableTokenAcquisitionToCallDownstreamApi(Array.Empty<string>())
      .AddInMemoryTokenCaches();;

//services.Configure<MicrosoftIdentityOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
//{
//    options.Events.OnTokenValidated = async context =>
//    {
//        if (applicationServices != null && context.Principal != null)
//        {
//            var test = "";
//        }
//    };

//});

services.AddControllersWithViews(options =>
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

services.AddRazorPages().AddMvcOptions(options =>
{
    //var policy = new AuthorizationPolicyBuilder()
    //    .RequireAuthenticatedUser()
    //    .Build();
    //options.Filters.Add(new AuthorizeFilter(policy));
}).AddMicrosoftIdentityUI();

var app = builder.Build();
applicationServices = app.Services;

if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseSecurityHeaders(
        SecurityHeadersDefinitions.GetHeaderPolicyCollection(env.IsDevelopment(),
            configuration["AzureB2C:Instance"]));

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseNoUnauthorizedRedirect("/api");

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapNotFound("/api/{**segment}");
app.MapFallbackToPage("/_Host");

app.Run();
