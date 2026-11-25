using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos;

public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Wachtwoord { get; set; } = string.Empty;
}
