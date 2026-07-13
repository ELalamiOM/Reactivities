using Application.Core;
using Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Microsoft.Extensions.Logging;
using Domain.Entities;

namespace Application.Profiles.Commands;

public class DeletePhoto
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context, IProfilesService photoService, IUserAccessor userAccessor, ILogger<Handler> logger)
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = userAccessor.GetUserId();
            logger.LogInformation("User {UserId} attempting to delete photo: {PhotoId}", userId, request.Id);
            
            var user = await context.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);

            if (user == null)
            {
                logger.LogWarning("Unauthorized photo deletion attempt by user: {UserId}", userId);
                return Result<Unit>.Failure("Unauthorized", 401);
            }

            var photo = user.Photos.FirstOrDefault(x => x.Id == request.Id);

            if (photo == null)
            {
                logger.LogWarning("Photo not found for deletion: {PhotoId} by user {UserId}", request.Id, userId);
                return Result<Unit>.Failure("Photo not found", 404);
            }

            if (!string.IsNullOrEmpty(photo.PublicId))
            {
                logger.LogDebug("Deleting photo from Cloudinary: {PublicId}", photo.PublicId);
                await photoService.DeletePhotoAsync(photo.PublicId);
            }

            user.Photos.Remove(photo);

            var result = await context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                logger.LogError("Failed to delete photo from database: {PhotoId}", request.Id);
                return Result<Unit>.Failure("Problem deleting photo", 400);
            }

            logger.LogInformation("Photo deleted successfully: {PhotoId} by user {UserId}", request.Id, userId);
            return Result<Unit>.Success(Unit.Value);
        }
    }
}
