using alsafqa.Data.Entities;
using alsafqa.Repository.Interfaces;
using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace alsafqa.Service.Services;

public class GalleryService : IGalleryService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public GalleryService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<IReadOnlyList<GalleryDto>>> GetAllAsync(Guid? categoryId = null, bool? isActive = null)
    {
        var query = _unitOfWork.Repository<Gallery>().Query()
            .Include(g => g.GalleryCategory)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(g => g.GalleryCategoryId == categoryId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(g => g.IsActive == isActive.Value);
        }

        var galleries = await query.OrderByDescending(g => g.CreatedAt).ToListAsync();
        return ApiResponse<IReadOnlyList<GalleryDto>>.Ok(galleries.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<GalleryDto>> GetByIdAsync(Guid id)
    {
        var gallery = await _unitOfWork.Repository<Gallery>().Query()
            .Include(g => g.GalleryCategory)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (gallery is null)
        {
            return ApiResponse<GalleryDto>.Fail("Gallery item not found.");
        }

        return ApiResponse<GalleryDto>.Ok(MapToDto(gallery));
    }

    public async Task<ApiResponse<GalleryDto>> CreateAsync(CreateGalleryDto request)
    {
        var category = await _unitOfWork.Repository<GalleryCategory>()
            .FirstOrDefaultAsync(c => c.Id == request.GalleryCategoryId);

        if (category is null)
        {
            return ApiResponse<GalleryDto>.Fail("Gallery category not found.");
        }

        string? imageUrl = null;
        if (request.Image is not null)
        {
            var validationError = ValidateImage(request.Image);
            if (validationError is not null)
            {
                return ApiResponse<GalleryDto>.Fail(validationError);
            }

            imageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "gallery");
        }

        var gallery = new Gallery
        {
            Id = Guid.NewGuid(),
            TitleAr = request.TitleAr,
            TitleEn = request.TitleEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            ImageUrl = imageUrl,
            IsActive = request.IsActive,
            GalleryCategoryId = request.GalleryCategoryId
        };

        await _unitOfWork.Repository<Gallery>().AddAsync(gallery);
        await _unitOfWork.SaveChangesAsync();

        gallery.GalleryCategory = category;
        return ApiResponse<GalleryDto>.Ok(MapToDto(gallery), "Gallery item created successfully.");
    }

    public async Task<ApiResponse<GalleryDto>> UpdateAsync(Guid id, UpdateGalleryDto request)
    {
        var gallery = await _unitOfWork.Repository<Gallery>().Query()
            .Include(g => g.GalleryCategory)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (gallery is null)
        {
            return ApiResponse<GalleryDto>.Fail("Gallery item not found.");
        }

        var category = await _unitOfWork.Repository<GalleryCategory>()
            .FirstOrDefaultAsync(c => c.Id == request.GalleryCategoryId);

        if (category is null)
        {
            return ApiResponse<GalleryDto>.Fail("Gallery category not found.");
        }

        gallery.TitleAr = request.TitleAr;
        gallery.TitleEn = request.TitleEn;
        gallery.DescriptionAr = request.DescriptionAr;
        gallery.DescriptionEn = request.DescriptionEn;
        gallery.IsActive = request.IsActive;
        gallery.GalleryCategoryId = request.GalleryCategoryId;
        gallery.GalleryCategory = category;
        gallery.UpdatedAt = DateTime.UtcNow;

        if (request.Image is not null)
        {
            var validationError = ValidateImage(request.Image);
            if (validationError is not null)
            {
                return ApiResponse<GalleryDto>.Fail(validationError);
            }

            if (!string.IsNullOrWhiteSpace(gallery.ImageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(gallery.ImageUrl);
            }

            gallery.ImageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "gallery");
        }

        _unitOfWork.Repository<Gallery>().Update(gallery);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<GalleryDto>.Ok(MapToDto(gallery), "Gallery item updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var gallery = await _unitOfWork.Repository<Gallery>().FirstOrDefaultAsync(g => g.Id == id);
        if (gallery is null)
        {
            return ApiResponse<bool>.Fail("Gallery item not found.");
        }

        if (!string.IsNullOrWhiteSpace(gallery.ImageUrl))
        {
            await _cloudinaryService.DeleteImageAsync(gallery.ImageUrl);
        }

        gallery.IsDeleted = true;
        gallery.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Gallery>().Update(gallery);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Gallery item deleted successfully.");
    }

    private static string? ValidateImage(IFormFile file)
    {
        if (file.Length == 0)
        {
            return "Image file is empty.";
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            return "Only JPG, PNG, and WEBP images are allowed.";
        }

        return null;
    }

    private static GalleryDto MapToDto(Gallery gallery) => new()
    {
        Id = gallery.Id,
        TitleAr = gallery.TitleAr,
        TitleEn = gallery.TitleEn,
        DescriptionAr = gallery.DescriptionAr,
        DescriptionEn = gallery.DescriptionEn,
        ImageUrl = gallery.ImageUrl,
        IsActive = gallery.IsActive,
        GalleryCategoryId = gallery.GalleryCategoryId,
        CategoryNameAr = gallery.GalleryCategory?.NameAr ?? string.Empty,
        CategoryNameEn = gallery.GalleryCategory?.NameEn ?? string.Empty
    };
}
