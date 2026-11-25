using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using PortfolioApi.Data;
using PortfolioApi.Data.Entities;

namespace PortfolioApi.Tests;

// Start de echte API in het geheugen, met de echte Identity-configuratie.
// De omgeving "Testing" gebruikt appsettings.Testing.json: een aparte database PortfolioDb_Test,
// zodat de tests de gewone database PortfolioDb nooit aanraken.
public class PortfolioApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
    }

    // Vóór alle tests: testdatabase leeggooien en opnieuw opbouwen (migraties + rollen).
    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await context.Database.EnsureDeletedAsync();
        await RolSeeder.SeedAsync(Services);
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;

    // Een client zonder aanmelding. De client bewaart cookies, net als een browser.
    public HttpClient MaakClient()
    {
        return CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
    }

    // Registreert een nieuwe gebruiker (krijgt Gast), geeft eventueel een extra rol en logt in.
    // Elke keer een nieuw e-mailadres en een willekeurig wachtwoord: er staan geen vaste wachtwoorden in de code.
    public async Task<HttpClient> MaakIngelogdeClientAsync(string? extraRol = null)
    {
        var client = MaakClient();
        var email = $"{Guid.NewGuid():N}@test.nl";
        var wachtwoord = MaakWachtwoord();

        var register = await client.PostAsJsonAsync("/api/auth/register", new { email, wachtwoord });
        register.EnsureSuccessStatusCode();

        if (extraRol is not null)
        {
            using var scope = Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync(email);
            await userManager.AddToRoleAsync(user!, extraRol);
        }

        // Inloggen ná de roltoekenning, want de rollen worden in de cookie opgeslagen.
        var login = await client.PostAsJsonAsync("/api/auth/login", new { email, wachtwoord });
        login.EnsureSuccessStatusCode();

        return client;
    }

    // Voldoet aan de standaard wachtwoordregels van Identity (hoofdletter, kleine letter, cijfer, teken).
    public static string MaakWachtwoord() => $"Aa1!{Guid.NewGuid():N}";
}

// Alle testklassen delen één factory en draaien na elkaar (niet tegelijk op dezelfde testdatabase).
[CollectionDefinition("Api")]
public class ApiCollection : ICollectionFixture<PortfolioApiFactory>
{
}
