using Microsoft.EntityFrameworkCore;
using PortfolioApi.Data;
using PortfolioApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ---- 1. Services registreren (vóór Build) ----
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger services (Les 5)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Database (EF Core + SQL Server)
builder.Services.AddDbContext<PortfolioDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PortfolioDb")));

// Services (applicatielogica)
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IBlogpostService, BlogpostService>();

var app = builder.Build();

// ---- 2. Middleware pipeline (na Build) ----
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();