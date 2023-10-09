using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.API.Data;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<MoviesContext>(options =>
    options.UseInMemoryDatabase(("MoviesDb") ?? throw new InvalidOperationException("Connection string 'MoviesAPIContext' not found.")));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:5005";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
        };
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization(options =>
{
    //options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient"));
    options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient", "movies_mvc_client"));  // you can remove the movieClient since we have commentted it out in the config
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
SeedDatabase(app);

void SeedDatabase(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var moviesContext = services.GetRequiredService<MoviesContext>();
    MoviesContextSeed.SeedAsync(moviesContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
