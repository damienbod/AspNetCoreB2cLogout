using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace RazorB2cLogout;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApp(authOptions => { builder.Configuration.Bind("AzureAd", authOptions); },
            sessionOptions =>
            {
                sessionOptions.Events.OnCheckSlidingExpiration = CookieEventHandler.SlidingExpirationAsync;
            })
            .EnableTokenAcquisitionToCallDownstreamApi(Array.Empty<string>())
            .AddDistributedTokenCaches();

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        builder.Services.AddRazorPages()
        .AddMvcOptions(options =>
        {
            options.Filters.Add(new SessionTimeoutAsyncPageFilter());
        })
        .AddMicrosoftIdentityUI();

        var app = builder.Build();

        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();
        app.MapControllers();

        app.Run();
    }
}