using Application.Activities.Queries;
using Application.Core;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Application;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers( opt =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    opt.Filters.Add(new AuthorizeFilter(policy));
});
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddMediatR(x =>
             x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>());
//builder.Services.AddAutoMapper(typeof(MappingProfiles).Assembly);
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfiles).Assembly);

builder.Services.AddOpenApi();
builder.Services.AddIdentityApiEndpoints<User>( opt =>
{
    opt.User.RequireUniqueEmail = true;
} )
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();
var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
   .AllowCredentials()
   .WithOrigins("http://localhost:3000","https://localhost:3000")
   );
   
app.UseAuthentication();
app.UseAuthorization();

app.MapOpenApi();
app.MapScalarApiReference();

app.MapControllers();
app.MapGroup("api").MapIdentityApi<User>(); // api/login

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<User>>();
    await context.Database.MigrateAsync();
    await DbInitializer.SeedData(context,userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred during migration");
    throw;
}

app.Run();
