using alsafqa.Data.Entities;
using alsafqa.Repository.Interfaces;
using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace alsafqa.Service.Services;

public class GalleryCategoryService : IGalleryCategoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public GalleryCategoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IReadOnlyList<GalleryCategoryDto>>> GetAllAsync(bool? isActive = null)
    {
        var query = _unitOfWork.Repository<GalleryCategory>().Query()
            .Include(c => c.Galleries)
            .AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(c => c.IsActive == isActive.Value);
        }

        var categories = await query.OrderBy(c => c.NameAr).ToListAsync();
        return ApiResponse<IReadOnlyList<GalleryCategoryDto>>.Ok(categories.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<GalleryCategoryDto>> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Repository<GalleryCategory>().Query()
            .Include(c => c.Galleries)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return ApiResponse<GalleryCategoryDto>.Fail("Gallery category not found.");
        }

        return ApiResponse<GalleryCategoryDto>.Ok(MapToDto(category));
    }

    public async Task<ApiResponse<GalleryCategoryDto>> CreateAsync(CreateGalleryCategoryDto request)
    {
        var category = new GalleryCategory
        {
            Id = Guid.NewGuid(),
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            IsActive = request.IsActive
        };

        await _unitOfWork.Repository<GalleryCategory>().AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<GalleryCategoryDto>.Ok(MapToDto(category), "Gallery category created successfully.");
    }

    public async Task<ApiResponse<GalleryCategoryDto>> UpdateAsync(Guid id, UpdateGalleryCategoryDto request)
    {
        var category = await _unitOfWork.Repository<GalleryCategory>().FirstOrDefaultAsync(c => c.Id == id);
        if (category is null)
        {
            return ApiResponse<GalleryCategoryDto>.Fail("Gallery category not found.");
        }

        category.NameAr = request.NameAr;
        category.NameEn = request.NameEn;
        category.IsActive = request.IsActive;
        category.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<GalleryCategory>().Update(category);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<GalleryCategoryDto>.Ok(MapToDto(category), "Gallery category updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var category = await _unitOfWork.Repository<GalleryCategory>().Query()
            .Include(c => c.Galleries)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (category is null)
        {
            return ApiResponse<bool>.Fail("Gallery category not found.");
        }

        if (category.Galleries.Any())
        {
            return ApiResponse<bool>.Fail("Cannot delete category that has gallery items. Delete items first.");
        }

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<GalleryCategory>().Update(category);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Gallery category deleted successfully.");
    }

    private static GalleryCategoryDto MapToDto(GalleryCategory category) => new()
    {
        Id = category.Id,
        NameAr = category.NameAr,
        NameEn = category.NameEn,
        IsActive = category.IsActive,
        GalleriesCount = category.Galleries?.Count ?? 0
    };
}
