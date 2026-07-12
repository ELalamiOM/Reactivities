using System;

namespace Application.Profiles.DTOs;

public class PhotoUploadResult
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}