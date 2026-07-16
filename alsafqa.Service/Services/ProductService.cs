using alsafqa.Data.Entities;
using alsafqa.Repository.Interfaces;
using alsafqa.Service.DTOs;
using alsafqa.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace alsafqa.Service.Services;

public class ProductService : IProductService
{
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICloudinaryService _cloudinaryService;

    public ProductService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
    {
        _unitOfWork = unitOfWork;
        _cloudinaryService = cloudinaryService;
    }

    public async Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllAsync(Guid? brandId = null, bool? isActive = null)
    {
        var query = _unitOfWork.Repository<Product>().Query()
            .Include(p => p.Brand)
            .AsQueryable();

        if (brandId.HasValue)
        {
            query = query.Where(p => p.BrandId == brandId.Value);
        }

        if (isActive.HasValue)
        {
            query = query.Where(p => p.IsActive == isActive.Value);
        }

        var products = await query.OrderBy(p => p.NameAr).ToListAsync();
        return ApiResponse<IReadOnlyList<ProductDto>>.Ok(products.Select(MapToDto).ToList());
    }

    public async Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Repository<Product>().Query()
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return ApiResponse<ProductDto>.Fail("Product not found.");
        }

        return ApiResponse<ProductDto>.Ok(MapToDto(product));
    }

    public async Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto request)
    {
        var brand = await _unitOfWork.Repository<Brand>().FirstOrDefaultAsync(b => b.Id == request.BrandId);
        if (brand is null)
        {
            return ApiResponse<ProductDto>.Fail("Brand not found.");
        }

        string? imageUrl = null;

        if (request.Image is not null)
        {
            var validationError = ValidateImage(request.Image);
            if (validationError is not null)
            {
                return ApiResponse<ProductDto>.Fail(validationError);
            }

            imageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "products");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            NameAr = request.NameAr,
            NameEn = request.NameEn,
            DescriptionAr = request.DescriptionAr,
            DescriptionEn = request.DescriptionEn,
            WeightInGrams = request.WeightInGrams,
            PackageCount = request.PackageCount,
            ImageUrl = imageUrl,
            IsActive = request.IsActive,
            BrandId = request.BrandId
        };

        await _unitOfWork.Repository<Product>().AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        product.Brand = brand;
        return ApiResponse<ProductDto>.Ok(MapToDto(product), "Product created successfully.");
    }

    public async Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, UpdateProductDto request)
    {
        var product = await _unitOfWork.Repository<Product>().Query()
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            return ApiResponse<ProductDto>.Fail("Product not found.");
        }

        var brand = await _unitOfWork.Repository<Brand>().FirstOrDefaultAsync(b => b.Id == request.BrandId);
        if (brand is null)
        {
            return ApiResponse<ProductDto>.Fail("Brand not found.");
        }

        product.NameAr = request.NameAr;
        product.NameEn = request.NameEn;
        product.DescriptionAr = request.DescriptionAr;
        product.DescriptionEn = request.DescriptionEn;
        product.WeightInGrams = request.WeightInGrams;
        product.PackageCount = request.PackageCount;
        product.IsActive = request.IsActive;
        product.BrandId = request.BrandId;
        product.Brand = brand;
        product.UpdatedAt = DateTime.UtcNow;

        if (request.Image is not null)
        {
            var validationError = ValidateImage(request.Image);
            if (validationError is not null)
            {
                return ApiResponse<ProductDto>.Fail(validationError);
            }

            if (!string.IsNullOrWhiteSpace(product.ImageUrl))
            {
                await _cloudinaryService.DeleteImageAsync(product.ImageUrl);
            }

            product.ImageUrl = await _cloudinaryService.UploadImageAsync(request.Image, "products");
        }

        _unitOfWork.Repository<Product>().Update(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<ProductDto>.Ok(MapToDto(product), "Product updated successfully.");
    }

    public async Task<ApiResponse<bool>> DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Repository<Product>().FirstOrDefaultAsync(p => p.Id == id);
        if (product is null)
        {
            return ApiResponse<bool>.Fail("Product not found.");
        }

        if (!string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            await _cloudinaryService.DeleteImageAsync(product.ImageUrl);
        }

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Repository<Product>().Update(product);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Product deleted successfully.");
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

    private static ProductDto MapToDto(Product product) => new()
    {
        Id = product.Id,
        NameAr = product.NameAr,
        NameEn = product.NameEn,
        DescriptionAr = product.DescriptionAr,
        DescriptionEn = product.DescriptionEn,
        WeightInGrams = product.WeightInGrams,
        PackageCount = product.PackageCount,
        ImageUrl = product.ImageUrl,
        IsActive = product.IsActive,
        BrandId = product.BrandId,
        BrandNameAr = product.Brand?.NameAr ?? string.Empty,
        BrandNameEn = product.Brand?.NameEn ?? string.Empty
    };
}
