using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace alsafqa.Service.DTOs;

public class UpdateBrandDto
{
    [Required]
    [MaxLength(200)]
    public string NameAr { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string NameEn { get; set; } = string.Empty;

    public IFormFile? Image { get; set; }

    public bool IsActive { get; set; } = true;
}
