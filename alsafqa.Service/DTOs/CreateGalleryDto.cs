using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace alsafqa.Service.DTOs;

public class CreateGalleryDto
{
    [Required]
    [MaxLength(300)]
    public string TitleAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(300)]
    public string TitleEn { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? DescriptionAr { get; set; }

    [MaxLength(1000)]
    public string? DescriptionEn { get; set; }

    public IFormFile? Image { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public Guid GalleryCategoryId { get; set; }
}
