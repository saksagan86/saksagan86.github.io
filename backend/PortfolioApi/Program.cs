using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Auth;
using PortfolioApi.Data;
using PortfolioApi.Data.Entities;
using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ----  Services registreren  
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger services 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Database (EF Core + SQL Server)
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortfolioDb")));

// Identity (Les 7): gebruikers en rollen opgeslagen via EF Core.
// AddIdentityApiEndpoints zorgt dat de API 401/403 teruggeeft in plaats van door te sturen naar een loginpagina.
builder.Services.AddIdentityApiEndpoints<User>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PortfolioDbContext>();

// Autorisatie: alleen Editor en Admin mogen schrijven. Admin staat er expliciet bij.
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.KanBewerken, policy => policy.RequireRole(Rollen.Editor, Rollen.Admin));

// Services (applicatielogica)
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogpostService, BlogpostService>();

var app = builder.Build();

// Migraties toepassen en de rollen Gast, Editor en Admin aanmaken
await RolSeeder.SeedAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
