using Application.Profiles.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IProfilesService
{
    Task<PhotoUploadResult> UploadPhotoAsync(IFormFile file);
    Task<string> DeletePhotoAsync(string publicId);
}
