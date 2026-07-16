using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IGalleryService
{
    Task<ApiResponse<IReadOnlyList<GalleryDto>>> GetAllAsync(Guid? categoryId = null, bool? isActive = null);
    Task<ApiResponse<GalleryDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<GalleryDto>> CreateAsync(CreateGalleryDto request);
    Task<ApiResponse<GalleryDto>> UpdateAsync(Guid id, UpdateGalleryDto request);
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
