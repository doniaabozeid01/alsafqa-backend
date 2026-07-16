using alsafqa.Data.Entities;
using alsafqa.Repository.Interfaces;
using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace alsafqa.Service.Services;

public class BrandService : IBrandService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public BrandService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<IReadOnlyList<BrandDto>>> GetAllAsync(bool? isActive = null)
    {
        var query = _unitOfWork.Repository<Brand>().Query()
            .Include(b => b.Products)
            .AsQueryable();

        if (isActive.HasValue)
        {
            query = query.Where(b => b.IsActive == isActive.Value);
        }

        var brands = await query.OrderBy(b => b.NameAr).ToListAsync();
        return ApiResponse<IReadOnlyList<BrandDto>>.Ok(brands.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<BrandDetailDto>> GetByIdAsync(Guid id)
    {
        var brand = await _unitOfWork.Repository<Brand>().Query()
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand is null)
        {
            return ApiResponse<BrandDetailDto>.Fail("Brand not found.");
        }

        return ApiResponse<BrandDetailDto>.Ok(MapToDetailDto(brand));
    }

    public async Task<ApiResponse<BrandDto>> CreateAsync(CreateBrandDto request)
    {
        string? imageUrl = null;

        if (request.Image is not null)
        {
            var validationError = ValidateImage(request.Image);
            if (validationError is not null)
            {
                return ApiResponse<BrandDto>.Fail(validationError);
            }

            imageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "brands");
        }

        var brand = new Brand
        {
            Id = Guid.NewGuid(),
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            ImageUrl = imageUrl,
            IsActive = request.IsActive
        };

        await _unitOfWork.Repository<Brand>().AddAsync(brand);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<BrandDto>.Ok(MapToDto(brand), "Brand created successfully.");
    }

    public async Task<ApiResponse<BrandDto>> UpdateAsync(Guid id, UpdateBrandDto request)
    {
        var brand = await _unitOfWork.Repository<Brand>().FirstOrDefaultAsync(b => b.Id == id);
        if (brand is null)
        {
            return ApiResponse<BrandDto>.Fail("Brand not found.");
        }

        brand.NameAr = request.NameAr;
        brand.NameEn = request.NameEn;
        brand.IsActive = request.IsActive;
        brand.UpdatedAt = DateTime.UtcNow;

        if (request.Image is not null)
        {
            var validationError = ValidateImage(request.Image);
            if (validationError is not null)
            {
                return ApiResponse<BrandDto>.Fail(validationError);
            }

            if (!string.IsNullOrWhiteSpace(brand.ImageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(brand.ImageUrl);
            }

            brand.ImageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "brands");
        }

        _unitOfWork.Repository<Brand>().Update(brand);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<BrandDto>.Ok(MapToDto(brand), "Brand updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var brand = await _unitOfWork.Repository<Brand>().Query()
            .Include(b => b.Products)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (brand is null)
        {
            return ApiResponse<bool>.Fail("Brand not found.");
        }

        if (brand.Products.Any())
        {
            return ApiResponse<bool>.Fail("Cannot delete brand that has products. Delete products first.");
        }

        if (!string.IsNullOrWhiteSpace(brand.ImageUrl))
        {
            await _cloudinaryService.DeleteImageAsync(brand.ImageUrl);
        }

        brand.IsDeleted = true;
        brand.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Brand>().Update(brand);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Brand deleted successfully.");
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

    private static BrandDto MapToDto(Brand brand) => new()
    {
        Id = brand.Id,
        NameAr = brand.NameAr,
        NameEn = brand.NameEn,
        ImageUrl = brand.ImageUrl,
        IsActive = brand.IsActive,
        ProductsCount = brand.Products?.Count ?? 0
    };

    private static BrandDetailDto MapToDetailDto(Brand brand) => new()
    {
        Id = brand.Id,
        NameAr = brand.NameAr,
        NameEn = brand.NameEn,
        ImageUrl = brand.ImageUrl,
        IsActive = brand.IsActive,
        Products = brand.Products.Select(p => new ProductDto
        {
            Id = p.Id,
            NameAr = p.NameAr,
            NameEn = p.NameEn,
            DescriptionAr = p.DescriptionAr,
            DescriptionEn = p.DescriptionEn,
            WeightInGrams = p.WeightInGrams,
            PackageCount = p.PackageCount,
            ImageUrl = p.ImageUrl,
            IsActive = p.IsActive,
            BrandId = p.BrandId,
            BrandNameAr = brand.NameAr,
            BrandNameEn = brand.NameEn
        }).ToList()
    };
}
