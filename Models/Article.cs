using System.ComponentModel.DataAnnotations;

namespace Lab06.Models;

public class Article : BaseEntity
{
    [Required]
    [MinLength(5)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Introduceti continutul")]
    [MinLength(20, ErrorMessage = "Continutul articolului trebuie sa aiba cel putin 20 de caractere.")]
    public string Content { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime PublishedAt { get; set; } = DateTime.Now;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string? AuthorId { get; set; }
    public ApplicationUser? Author { get; set; }

    public string? ImagePath { get; set; }
}
