var builder = WebApplication.CreateBuilder(args);

// ---- 1. Services registreren (vóór Build) ----
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger services (Les 5)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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