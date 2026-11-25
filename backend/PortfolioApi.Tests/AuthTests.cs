using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Auth;
using PortfolioApi.Dtos;

namespace PortfolioApi.Tests;

[Collection("Api")]
public class AuthTests
{
    private readonly PortfolioApiFactory _factory;

    public AuthTests(PortfolioApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_NieuweGebruiker_KrijgtRolGast()
    {
        // Arrange
        var client = _factory.MaakClient();
        var email = $"{Guid.NewGuid():N}@test.nl";

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { email, wachtwoord = PortfolioApiFactory.MaakWachtwoord() });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var gebruiker = await response.Content.ReadFromJsonAsync<GebruikerDto>();
        Assert.Equal(email, gebruiker!.Email);
        Assert.Equal(new List<string> { Rollen.Gast }, gebruiker.Rollen);
    }

    [Fact]
    public async Task Register_ZwakWachtwoord_Geeft400()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register",
            new { email = $"{Guid.NewGuid():N}@test.nl", wachtwoord = "123" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_JuisteGegevens_GeeftCookieEnIsIngelogdAlsGast()
    {
        // Arrange
        var client = _factory.MaakClient();
        var email = $"{Guid.NewGuid():N}@test.nl";
        var wachtwoord = PortfolioApiFactory.MaakWachtwoord();
        await client.PostAsJsonAsync("/api/auth/register", new { email, wachtwoord });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, wachtwoord });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"),
            cookie => cookie.StartsWith(".AspNetCore.Identity.Application="));

        var ik = await client.GetFromJsonAsync<GebruikerDto>("/api/auth/me");
        Assert.Equal(email, ik!.Email);
        Assert.Contains(Rollen.Gast, ik.Rollen);
    }

    [Fact]
    public async Task Login_VerkeerdWachtwoord_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();
        var email = $"{Guid.NewGuid():N}@test.nl";
        await client.PostAsJsonAsync("/api/auth/register",
            new { email, wachtwoord = PortfolioApiFactory.MaakWachtwoord() });

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login",
            new { email, wachtwoord = "VerkeerdWachtwoord1!" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
