namespace alsafqa.Data.Entities;

public class GalleryCategory : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Gallery> Galleries { get; set; } = new List<Gallery>();
}
