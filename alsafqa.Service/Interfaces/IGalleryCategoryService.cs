using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IGalleryCategoryService
{
    Task<ApiResponse<IReadOnlyList<GalleryCategoryDto>>> GetAllAsync(bool? isActive = null);
    Task<ApiResponse<GalleryCategoryDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<GalleryCategoryDto>> CreateAsync(CreateGalleryCategoryDto request);
    Task<ApiResponse<GalleryCategoryDto>> UpdateAsync(Guid id, UpdateGalleryCategoryDto request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
