using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IContactMessageService
{
    Task<ApiResponse<ContactMessageDto>> CreateAsync(CreateContactMessageDto request);
    Task<ApiResponse<IReadOnlyList<ContactMessageDto>>> GetAllAsync(bool? isRead = null);
    Task<ApiResponse<ContactMessageDto>> GetByIdAsync(Guid id);
    Task<ApiResponse<ContactMessageDto>> MarkAsReadAsync(Guid id);
    Task<ApiResponse<ContactMessageDto>> MarkAsUnreadAsync(Guid id);
    Task<ApiResponse<int>> GetUnreadCountAsync();
    Task<ApiResponse<bool>> DeleteAsync(Guid id);
}
