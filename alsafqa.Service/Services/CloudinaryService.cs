using alsafqa.Service.Interfaces;
using alsafqa.Service.Settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace alsafqa.Service.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> options)
    {
        var settings = options.Value;
        var account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(IFormFile file, string folder)
    {
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"alsafqa/{folder}",
            Transformation = new Transformation()
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
        }

        return result.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return false;
        }

        var publicId = ExtractPublicId(imageUrl);
        if (string.IsNullOrWhiteSpace(publicId))
        {
            return false;
        }

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);
        return result.Result == "ok";
    }

    private static string? ExtractPublicId(string imageUrl)
    {
        try
        {
            var uri = new Uri(imageUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var uploadIndex = Array.FindIndex(segments, s => s.Equals("upload", StringComparison.OrdinalIgnoreCase));

            if (uploadIndex < 0 || uploadIndex + 1 >= segments.Length)
            {
                return null;
            }

            // Skip version segment (v123456) if present
            var startIndex = uploadIndex + 1;
            if (segments[startIndex].StartsWith("v", StringComparison.OrdinalIgnoreCase)
                && segments[startIndex].Length > 1
                && char.IsDigit(segments[startIndex][1]))
            {
                startIndex++;
            }

            var publicIdWithExt = string.Join('/', segments.Skip(startIndex));
            var lastDot = publicIdWithExt.LastIndexOf('.');
            return lastDot > 0 ? publicIdWithExt[..lastDot] : publicIdWithExt;
        }
        catch
        {
            return null;
        }
    }
}
