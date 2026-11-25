namespace PortfolioApi.Dtos;

// Wat de API over een gebruiker teruggeeft. Nooit het wachtwoord of de hash.
public class GebruikerDto
{
    public string Email { get; set; } = string.Empty;
    public List<string> Rollen { get; set; } = new();
}
