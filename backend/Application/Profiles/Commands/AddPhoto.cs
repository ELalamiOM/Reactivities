using Application.Core;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace Application.Profiles.Commands;

public class AddPhoto
{
    public class Command : IRequest<Result<Photo>>
    {
        public required IFormFile File { get; set; }
    }

    public class Handler(AppDbContext context, IProfilesService photoService, IUserAccessor userAccessor, ILogger<Handler> logger)
        : IRequestHandler<Command, Result<Photo>>
    {
        public async Task<Result<Photo>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = userAccessor.GetUserId();
            logger.LogInformation("User {UserId} uploading photo: {FileName}, Size: {Size} bytes", 
                userId, request.File.FileName, request.File.Length);
            
            var user = await context.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("Unauthorized photo upload attempt by user: {UserId}", userId);
                return Result<Photo>.Failure("Unauthorized", 401);
            }

            logger.LogDebug("Uploading photo to Cloudinary for user: {UserId}", userId);
            var uploadResult = await photoService.UploadPhotoAsync(request.File);

            var photo = new Photo
            {
                Url = uploadResult.Url,
                PublicId = uploadResult.PublicId
            };

            user.Photos.Add(photo);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                logger.LogError("Failed to save photo to database for user: {UserId}", userId);
                return Result<Photo>.Failure("Problem uploading photo", 400);
            }

            logger.LogInformation("Photo uploaded successfully: {PhotoId} for user {UserId}, URL: {Url}", 
                photo.Id, userId, photo.Url);
            return Result<Photo>.Success(photo);
        }
    }
}
