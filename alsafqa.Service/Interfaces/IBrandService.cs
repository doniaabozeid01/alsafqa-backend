using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IBrandService
{
    Task<ApiResponse<IReadOnlyList<BrandDto>>> GetAllAsync(bool? isActive = null);
    Task<ApiResponse<BrandDetailDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<BrandDto>> CreateAsync(CreateBrandDto request);
    Task<ApiResponse<BrandDto>> UpdateAsync(Guid id, UpdateBrandDto request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
