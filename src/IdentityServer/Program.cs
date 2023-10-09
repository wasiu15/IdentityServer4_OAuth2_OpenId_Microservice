using IdentityServer;
using IdentityServer4;
using IdentityServerHost.Quickstart.UI;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddIdentityServer()
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    //.AddTestUsers(Config.TestUsers)
    .AddTestUsers(TestUsers.Users)
    .AddDeveloperSigningCredential();

// Add the following lines to set SameSite=None for cookies
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
//    options.OnAppendCookie = cookieContext =>
//    {
//        if (cookieContext.CookieName == IdentityServerConstants.DefaultCookieAuthenticationScheme)
//        {
//            cookieContext.CookieOptions.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
//        }
//    };
//});

var app = builder.Build();
app.UseStaticFiles();
app.UseIdentityServer();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});
app.Run();