using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile($"ocelot.json");

var authenticationProviderKey = "IdentityApiKey";

builder.Services.AddAuthentication()
    .AddJwtBearer(authenticationProviderKey, x =>
    {
        x.Authority = "https://localhost:5005"; // IDENTITY SERVER URL
        //x.RequireHttpMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

await app.UseOcelot();

app.MapGet("/", () => "Hello World!");

app.Run();
