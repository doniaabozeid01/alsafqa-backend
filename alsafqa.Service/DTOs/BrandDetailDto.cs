namespace alsafqa.Service.DTOs;

public class BrandDetailDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public IList<ProductDto> Products { get; set; } = new List<ProductDto>();
}
