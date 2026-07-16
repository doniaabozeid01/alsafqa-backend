using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace alsafqa.Service.DTOs;

public class CreateProductDto
{
    [Required]
    [MaxLength(300)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string NameEn { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? DescriptionAr { get; set; }

    [MaxLength(1000)]
    public string? DescriptionEn { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal WeightInGrams { get; set; }

    [Range(1, int.MaxValue)]
    public int PackageCount { get; set; }

    public IFormFile? Image { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public Guid BrandId { get; set; }
}
