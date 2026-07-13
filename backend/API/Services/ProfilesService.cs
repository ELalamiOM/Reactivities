using Application.Interfaces;
using Application.Profiles.DTOs;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Infrastructure.Profiles.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;


namespace Infrastructure.Profiles;

public class ProfilesService : IProfilesService
{
    private readonly Cloudinary _cloudinary;

    public ProfilesService(IOptions<CloudinarySettings> cloudinaryOptions)
    {
        var cloudinarySettings = cloudinaryOptions.Value;
        var account = new Account(
            cloudinarySettings.CloudName,
            cloudinarySettings.ApiKey,
            cloudinarySettings.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }

    public async Task<PhotoUploadResult> UploadPhotoAsync(IFormFile file)
    {
        if (file.Length == 0)
            throw new InvalidOperationException("Photo file is empty");

        await using var stream = file.OpenReadStream();
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Transformation = new Transformation().Height(500).Width(500).Crop("fill")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error != null)
            throw new InvalidOperationException(result.Error.Message);

        return new PhotoUploadResult
        {
            PublicId = result.PublicId,
            Url = result.SecureUrl.ToString()
        };
    }

    public async Task<string> DeletePhotoAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            throw new ArgumentException("Public ID is required", nameof(publicId));

        var deleteParams = new DeletionParams(publicId);
        var result = await _cloudinary.DestroyAsync(deleteParams);

        return result.Result == "ok" ? result.Result
            : throw new InvalidOperationException("Failed to delete photo from Cloudinary");
    }
}