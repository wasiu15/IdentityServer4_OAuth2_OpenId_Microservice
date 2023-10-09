using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.Client.ApiServices;
using Microsoft.Net.Http.Headers;
using Movies.Client.HttpHandlers;
using IdentityModel.Client;
using IdentityModel;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMovieApiServices, MovieApiServices>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Authority = "https://localhost:5005";

    options.ClientId = "movies_mvc_client";
    options.ClientSecret = "secret";
    //options.ResponseType = "code";
    options.ResponseType = "code id_token"; // for hybrid

    //options.Scope.Add("openid");  // these two are automatically added in the background by OpenId
    //options.Scope.Add("profile"); // so it is not necessary to add it again here though adding it again wont break the code
    options.Scope.Add("movieAPI"); // for hybrid
    options.Scope.Add("address");
    options.Scope.Add("email");
    options.Scope.Add("roles");
    options.ClaimActions.MapUniqueJsonKey("role", "role");

    //options.ClaimActions.DeleteClaim("sid");
    //options.ClaimActions.DeleteClaim("idp");
    //options.ClaimActions.DeleteClaim("s_hash");
    //options.ClaimActions.DeleteClaim("auth_time");

    //options.Scope.Add("movieAPI");

    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters = new TokenValidationParameters   //this will confirm that these claims are available in the token else it will throw error
    {
        NameClaimType = JwtClaimTypes.GivenName,
        RoleClaimType = JwtClaimTypes.Role
    };
});


///////////////////
//  1 create an HttpClient used for accessing the Movies.API
builder.Services.AddTransient<AuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("MovieAPIClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5010/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

//  2 create an HttpClient used for accessig the IDP
builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5005/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
});



builder.Services.AddHttpContextAccessor();  // for hybrid ===> we we will get the existing token from this instead of generating a new one since we've added the two scopes together
//builder.Services.AddSingleton(new ClientCredentialsTokenRequest
//{
//    Address = "https://localhost:5005/connect/token",
//    ClientId = "movieClient",
//    ClientSecret = "secret",
//    Scope = "movieAPI"
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
