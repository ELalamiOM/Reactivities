using Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
{
    public required DbSet<Activity> Activities { get; set; }
    public required DbSet<ActivityAttendee> ActivityAttendees { get; set; }
    public required DbSet<Photo> Photos { get; set; }
    public required DbSet<PrepaidAccount> PrepaidAccounts { get; set; }
    public required DbSet<AccountTransaction> AccountTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ActivityAttendee>(x => x.HasKey(a => new { a.ActivityId, a.UserId }));

        builder.Entity<ActivityAttendee>()
            .HasOne(x => x.User)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.UserId);

        builder.Entity<ActivityAttendee>()
            .HasOne(x => x.Activity)
            .WithMany(x => x.Attendees)
            .HasForeignKey(x => x.ActivityId);

        builder.Entity<PrepaidAccount>()
    .HasOne(x => x.User)
    .WithOne()
    .HasForeignKey<PrepaidAccount>(x => x.UserId);

         builder.Entity<AccountTransaction>()
    .HasOne(x => x.Account)
    .WithMany(x => x.Transactions)
    .HasForeignKey(x => x.AccountId);
    
    }
}