using System;

namespace Domain.Entities;


public class Photo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Url { get; set; } = string.Empty;
    public string PublicId { get; set; } = string.Empty;
}