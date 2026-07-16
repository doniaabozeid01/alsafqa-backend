using alsafqa.Service.DTOs;

namespace alsafqa.Service.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId);
    Task<ApiResponse<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto request);
}
