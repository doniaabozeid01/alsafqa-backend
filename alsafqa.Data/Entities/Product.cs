namespace alsafqa.Data.Entities;

public class Product : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public decimal WeightInGrams { get; set; }
    public int PackageCount { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid BrandId { get; set; }
    public Brand Brand { get; set; } = null!;
}
