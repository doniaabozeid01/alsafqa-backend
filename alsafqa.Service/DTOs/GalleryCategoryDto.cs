namespace alsafqa.Service.DTOs;

public class GalleryCategoryDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int GalleriesCount { get; set; }
}
