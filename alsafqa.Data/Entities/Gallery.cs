namespace alsafqa.Data.Entities;

public class Gallery : BaseEntity
{
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string? DescriptionAr { get; set; }
    public string? DescriptionEn { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public Guid GalleryCategoryId { get; set; }
    public GalleryCategory GalleryCategory { get; set; } = null!;
}
