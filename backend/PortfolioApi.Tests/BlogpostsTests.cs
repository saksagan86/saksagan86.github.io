using System.Net;
using System.Net.Http.Json;
using PortfolioApi.Auth;
using PortfolioApi.Dtos;

namespace PortfolioApi.Tests;

[Collection("Api")]
public class BlogpostsTests
{
    private const string Url = "/api/blogposts";
    private readonly PortfolioApiFactory _factory;

    public BlogpostsTests(PortfolioApiFactory factory)
    {
        _factory = factory;
    }

    private static CreateBlogpostDto NieuweBlogpost(string titel = "Testblog") => new()
    {
        Titel = titel,
        Inhoud = "Inhoud gemaakt door een test",
        Publicatiedatum = new DateTime(2026, 11, 25)
    };

    // Hulpmethode: maakt een blogpost aan als Editor en geeft hem terug.
    private async Task<BlogpostDto> MaakBlogpostAanAsync()
    {
        var editor = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);
        var response = await editor.PostAsJsonAsync(Url, NieuweBlogpost());
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<BlogpostDto>())!;
    }

    // ---------- GET (publiek) ----------

    [Fact]
    public async Task GetAll_ZonderAanmelding_Geeft200EnBevatBlogpost()
    {
        // Arrange
        var bestaand = await MaakBlogpostAanAsync();
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync(Url);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogposts = await response.Content.ReadFromJsonAsync<List<BlogpostDto>>();
        Assert.Contains(blogposts!, b => b.Id == bestaand.Id && b.Titel == bestaand.Titel);
    }

    [Fact]
    public async Task GetById_BestaandId_Geeft200EnJuisteBlogpost()
    {
        // Arrange
        var bestaand = await MaakBlogpostAanAsync();
        var client = _factory.MaakClient();

        // Act
        var response = await client.GetAsync($"{Url}/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var blogpost = await response.Content.ReadFromJsonAsync<BlogpostDto>();
        Assert.Equal(bestaand.Id, blogpost!.Id);
        Assert.Equal("Testblog", blogpost.Titel);
    }

    // ---------- POST ----------

    [Theory]
    [InlineData(Rollen.Editor)]
    [InlineData(Rollen.Admin)]
    public async Task Post_AlsEditorOfAdmin_Geeft201EnBlogpostIsOpgeslagen(string rol)
    {
        // Arrange
        var client = await _factory.MaakIngelogdeClientAsync(rol);

        // Act
        var response = await client.PostAsJsonAsync(Url, NieuweBlogpost($"Blog van {rol}"));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var aangemaakt = await response.Content.ReadFromJsonAsync<BlogpostDto>();
        var opgehaald = await client.GetFromJsonAsync<BlogpostDto>($"{Url}/{aangemaakt!.Id}");
        Assert.Equal($"Blog van {rol}", opgehaald!.Titel);
    }

    [Fact]
    public async Task Post_ZonderAanmelding_Geeft401()
    {
        // Arrange
        var client = _factory.MaakClient();

        // Act
        var response = await client.PostAsJsonAsync(Url, NieuweBlogpost());

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // ---------- PUT ----------

    [Fact]
    public async Task Put_AlsEditor_Geeft200EnWijzigingIsOpgeslagen()
    {
        // Arrange
        var bestaand = await MaakBlogpostAanAsync();
        var client = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);
        var wijziging = new UpdateBlogpostDto
        {
            Titel = "Gewijzigde blog",
            Inhoud = "Nieuwe inhoud",
            Publicatiedatum = bestaand.Publicatiedatum
        };

        // Act
        var response = await client.PutAsJsonAsync($"{Url}/{bestaand.Id}", wijziging);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var opgehaald = await client.GetFromJsonAsync<BlogpostDto>($"{Url}/{bestaand.Id}");
        Assert.Equal("Gewijzigde blog", opgehaald!.Titel);
        Assert.Equal("Nieuwe inhoud", opgehaald.Inhoud);
    }

    // ---------- DELETE ----------

    [Fact]
    public async Task Delete_AlsAdmin_Geeft204EnBlogpostIsWeg()
    {
        // Arrange
        var bestaand = await MaakBlogpostAanAsync();
        var admin = await _factory.MaakIngelogdeClientAsync(Rollen.Admin);

        // Act
        var response = await admin.DeleteAsync($"{Url}/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var opnieuw = await admin.GetAsync($"{Url}/{bestaand.Id}");
        Assert.Equal(HttpStatusCode.NotFound, opnieuw.StatusCode);
    }

    [Fact]
    public async Task Delete_OnbekendId_Geeft404()
    {
        // Arrange
        var editor = await _factory.MaakIngelogdeClientAsync(Rollen.Editor);

        // Act
        var response = await editor.DeleteAsync($"{Url}/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_AlsGast_Geeft403EnBlogpostBestaatNog()
    {
        // Arrange
        var bestaand = await MaakBlogpostAanAsync();
        var gast = await _factory.MaakIngelogdeClientAsync();

        // Act
        var response = await gast.DeleteAsync($"{Url}/{bestaand.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var opnieuw = await gast.GetAsync($"{Url}/{bestaand.Id}");
        Assert.Equal(HttpStatusCode.OK, opnieuw.StatusCode);
    }
}
