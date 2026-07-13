using System.Security.Claims;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class UserAccessor(IHttpContextAccessor httpContextAccessor, AppDbContext dbContext)
    : IUserAccessor
{
    public string GetUserId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No user is logged in");
    }

    public async Task<User> GetUserAsync()
    {
        var userId = GetUserId();
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId)
            ?? throw new UnauthorizedAccessException("No user is logged in");
    }
}
