namespace alsafqa.Service.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal WeightInGrams { get; set; }
    public int PackageCount { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public Guid BrandId { get; set; }
    public string BrandNameAr { get; set; } = string.Empty;
    public string BrandNameEn { get; set; } = string.Empty;
}
