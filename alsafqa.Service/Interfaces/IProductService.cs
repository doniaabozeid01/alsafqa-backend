using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IProductService
{
    Task<ApiResponse<IReadOnlyList<ProductDto>>> GetAllAsync(Guid? brandId = null, bool? isActive = null);
    Task<ApiResponse<ProductDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ProductDto>> CreateAsync(CreateProductDto request);
    Task<ApiResponse<ProductDto>> UpdateAsync(Guid id, UpdateProductDto request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
