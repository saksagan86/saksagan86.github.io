using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos;

public class CreateBlogpostDto
{
    [Required]
    [StringLength(100)]
    public string Titel { get; set; } = string.Empty;

    [Required]
    public string Inhoud { get; set; } = string.Empty;

    public DateTime Publicatiedatum { get; set; }
}