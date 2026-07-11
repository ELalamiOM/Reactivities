using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class DbInitializer
    {
        public static async Task SeedData(AppDbContext context,UserManager<User> userManager)
        {

            if(!userManager.Users.Any())
            {
                var users = new List<User>
                {
                    new() {DisplayName="Bob",UserName="bob@test.com",Email="bob@test.com"},
                    new() {DisplayName="Tom",UserName="tom@test.com",Email="tom@test.com"},
                    new() {DisplayName="Jane",UserName="jane@test.com",Email="jane@test.com"}
                };

                foreach(var user in users)
                {
                    await userManager.CreateAsync(user,"Pa$$w0rd");
                }
            }
            if (context.Activites.Any()) return;

            var members = await userManager.Users.ToListAsync();

            var activities = new List<Activity>
            {
                new()
                {
                    Title ="Test",
                    Date = DateTime.Now.AddMonths(8),
                    Description ="Activity 8 months in future",
                    Category = "File",
                    City ="London",
                    Venue="River Thales, England, United Kingdom",
                    Latitude = 51.5575525,
                    Longitude = -0.781404,
                    Attendees =
                    [
                        new() { User = members[0], IsHost = true }
                    ]
                },
                new()
                {
                    Title ="Test2",
                    Date = DateTime.Now.AddMonths(8),
                    Description ="Activity 8 months in future",
                    Category = "File",
                    City ="London",
                    Venue="River Thales, Monaco, france",
                    Latitude = 51.5575525,
                    Longitude = -0.781404,
                    Attendees =
                    [
                        new() { User = members[0], IsHost = true },
                        new() { User = members[1], IsHost = false }
                    ]
                },
                new()
                {
                    Title ="Test3",
                    Date = DateTime.Now.AddMonths(-2),
                    Description ="Activity 2 months in past",
                    Category = "File",
                    City ="London",
                    Venue="River Thales, American, Newyork",
                    Latitude = 51.5575525,
                    Longitude = -0.781404,
                    Attendees =
                    [
                        new() { User = members[1], IsHost = true },
                        new() { User = members[0], IsHost = false }
                    ]
                },
                new()
                {
                    Title ="Test4",
                    Date = DateTime.Now.AddMonths(-1),
                    Description ="Activity 1 month in past",
                    Category = "File",
                    City ="Lisbona",
                    Venue="River Thales, Portugal",
                    Latitude = 51.5575525,
                    Longitude = -0.781404,
                    Attendees =
                    [
                        new() { User = members[2], IsHost = true }
                    ]
                }
            };

            context.Activites.AddRange(activities);

            await context.SaveChangesAsync();

        }
    }
}
