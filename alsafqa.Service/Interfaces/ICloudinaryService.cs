using Microsoft.AspNetCore.Http;

namespace alsafqa.Service.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(IFormFile file, string folder);
    Task<bool> DeleteImageAsync(string imageUrl);
}
