using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Auth;

namespace PortfolioApi.Data;

// Wordt bij het opstarten aangeroepen in Program.cs.
// Past de migraties toe en zorgt dat de rollen Gast, Editor en Admin bestaan.
// Er worden hier geen gebruikers of wachtwoorden aangemaakt.
public static class RolSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        await using var scope = services.CreateAsyncScope();

        var context = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
        await context.Database.MigrateAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var rol in Rollen.Alle)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }
    }
}
