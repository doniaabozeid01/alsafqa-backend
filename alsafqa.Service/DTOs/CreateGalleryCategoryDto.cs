using System.ComponentModel.DataAnnotations;

namespace alsafqa.Service.DTOs;

public class CreateGalleryCategoryDto
{
    [Required]
    [MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
