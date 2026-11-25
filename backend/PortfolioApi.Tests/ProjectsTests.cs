using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Auth;
using PortfolioApi.Dtos;

namespace PortfolioApi.Tests;

[Collection("Api")]
public class ProjectsTests
{
    private const string Url = "/api/projects";
    private readonly PortfolioApiFactory _factory;

    public ProjectsTests(PortfolioApiFactory factory)
    {
        _factory = factory;
    }

    private static CreateProjectDto NieuwProject(string titel = "Testproject") => new()
    {
        Titel = titel,
        Beschrijving = "Gemaakt door een test",
        Categorie = "Backend",
        GitHubUrl = "https://github.com/saksagan86",
        Datum = new DateTime(2026, 11, 25)
    };

    // Hulpmethode: maakt een project aan als Editor en geeft het terug.
    private async Task<ProjectDto> MaakProjectAanAsync()
    {
        var editor = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);
        var response = await editor.PostAsJsonAsync(Url, NieuwProject());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProjectDto>())!;
    }

    // ---------- GET (publiek) ----------

    [Fact]
    public async Task GetAll_ZonderAanmelding_Geeft200EnBevatProject()
    {
        // Arrange
        var bestaand = await MaakProjectAanAsync();
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync(Url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var projecten = await response.Content.ReadFromJsonAsync<List<ProjectDto>>();
        Assert.Contains(projecten!, p => p.Id == bestaand.Id && p.Titel == bestaand.Titel);
    }

    [Fact]
    public async Task GetById_BestaandId_Geeft200EnJuisteProject()
    {
        // Arrange
        var bestaand = await MaakProjectAanAsync();
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync($"{Url}/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var project = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.Equal(bestaand.Id, project!.Id);
        Assert.Equal("Testproject", project.Titel);
        Assert.Equal("Backend", project.Categorie);
    }

    [Fact]
    public async Task GetById_OnbekendId_Geeft404()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync($"{Url}/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- POST ----------

    [Theory]
    [InlineData(Rollen.Editor)]
    [InlineData(Rollen.Admin)]
    public async Task Post_AlsEditorOfAdmin_Geeft201EnProjectIsOpgeslagen(string rol)
    {
        // Arrange
        var client = await _factory.MaakIngelogdeClientAsync(rol);

        // Act
        var response = await client.PostAsJsonAsync(Url, NieuwProject($"Project van {rol}"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var aangemaakt = await response.Content.ReadFromJsonAsync<ProjectDto>();
        Assert.True(aangemaakt!.Id > 0);

        var opgehaald = await client.GetFromJsonAsync<ProjectDto>($"{Url}/{aangemaakt.Id}");
        Assert.Equal($"Project van {rol}", opgehaald!.Titel);
    }

    [Fact]
    public async Task Post_ZonderAanmelding_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync(Url, NieuwProject());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Post_AlsGast_Geeft403()
    {
        // Arrange
        var gast = await _factory.MaakIngelogdeClientAsync();

        // Act
        var response = await gast.PostAsJsonAsync(Url, NieuwProject());

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Post_LegeTitel_Geeft400()
    {
        // Arrange
        var editor = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);
        var ongeldig = NieuwProject(titel: "");

        // Act
        var response = await editor.PostAsJsonAsync(Url, ongeldig);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    // ---------- PUT ----------

    [Fact]
    public async Task Put_AlsEditor_Geeft200EnWijzigingIsOpgeslagen()
    {
        // Arrange
        var bestaand = await MaakProjectAanAsync();
        var editor = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);
        var wijziging = new UpdateProjectDto
        {
            Titel = "Gewijzigde titel",
            Beschrijving = "Nieuwe beschrijving",
            Categorie = "Web",
            Datum = bestaand.Datum
        };

        // Act
        var response = await editor.PutAsJsonAsync($"{Url}/{bestaand.Id}", wijziging);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var opgehaald = await editor.GetFromJsonAsync<ProjectDto>($"{Url}/{bestaand.Id}");
        Assert.Equal("Gewijzigde titel", opgehaald!.Titel);
        Assert.Equal("Web", opgehaald.Categorie);
    }

    [Fact]
    public async Task Put_OnbekendId_Geeft404()
    {
        // Arrange
        var editor = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);
        var wijziging = new UpdateProjectDto { Titel = "X", Beschrijving = "X", Categorie = "X" };

        // Act
        var response = await editor.PutAsJsonAsync($"{Url}/999999", wijziging);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_AlsAdmin_Geeft204EnProjectIsWeg()
    {
        // Arrange
        var bestaand = await MaakProjectAanAsync();
        var admin = await _factory.MaakIngelogdeClientAsync(Rollen.Admin);

        // Act
        var response = await admin.DeleteAsync($"{Url}/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opnieuw = await admin.GetAsync($"{Url}/{bestaand.Id}");
        Assert.Equal(HttpStatusCode.NotFound, opnieuw.StatusCode);
    }
}
