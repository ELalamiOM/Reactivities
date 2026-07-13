using Application.Activities.Queries;
using Application.Core;
using Application.Interfaces;
using API.Services;
using Infrastructure.Persistence;
using Infrastructure.Profiles;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Domain.Entities;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using Application.Activities.Validators;
using Reactivity.Api.Middleware;
using Serilog;
using Microsoft.OpenApi;
using Infrastructure.Profiles.DTOs;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("logs/reactivities-.log", 
                rollingInterval: RollingInterval.Day,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        try
        {
            Log.Information("Starting Reactivities API...");
            
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Host.UseSerilog();

            builder.Services.AddControllers(opt =>
            {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            });

            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddCors();

            var jwtKey = builder.Configuration["JwtSettings:Key"]
                ?? throw new InvalidOperationException("JwtSettings:Key is not configured");
            var jwtIssuer = builder.Configuration["JwtSettings:Issuer"]
                ?? throw new InvalidOperationException("JwtSettings:Issuer is not configured");
            var jwtAudience = builder.Configuration["JwtSettings:Audience"]
                ?? throw new InvalidOperationException("JwtSettings:Audience is not configured");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = true,
                        ValidAudience = jwtAudience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddMediatR(x =>
            {
                x.RegisterServicesFromAssemblyContaining<GetActivityList.Handler>();
                x.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfiles).Assembly);
            builder.Services.AddValidatorsFromAssemblyContaining<CreateActivityValidator>();
            builder.Services.AddTransient<ExceptionMiddleware>();

            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddTransient<IProfilesService, ProfilesService>();
           //builder.Services.AddTransient<IProfilesService, ProfilesServiceAdapter>();

            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Enter your JWT token"
                    };

                    document.Security ??= [];
                    document.Security.Add(new OpenApiSecurityRequirement
                    {
                        [new OpenApiSecuritySchemeReference("Bearer", document, null)] = []
                    });

                    return Task.CompletedTask;
                });
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IUserAccessor, UserAccessor>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddIdentityCore<User>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<User>>()
            .AddDefaultTokenProviders();

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins("http://localhost:3000", "https://localhost:3000")
            );

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapOpenApi();
            app.MapScalarApiReference(options =>
            {
                options.WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
                options.Authentication = new ScalarAuthenticationOptions
                {
                    PreferredSecuritySchemes = ["Bearer"]
                };
            });
            app.MapControllers();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();
                await context.Database.MigrateAsync();
                await DbInitializer.SeedData(context, userManager);
                Log.Information("Database migration and seeding completed successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred during migration");
                throw;
            }

            Log.Information("Reactivities API started successfully");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}