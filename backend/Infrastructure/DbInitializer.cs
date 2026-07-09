using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

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
                    Longitude = -0.781404
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
                    Longitude = -0.781404
                },
                new()
                {
                    Title ="Test3",
                    Date = DateTime.Now.AddMonths(8),
                    Description ="Activity 8 months in future",
                    Category = "File",
                    City ="London",
                    Venue="River Thales, American, Newyork",
                    Latitude = 51.5575525,
                    Longitude = -0.781404
                },
                new()
                {
                    Title ="Test4",
                    Date = DateTime.Now.AddMonths(8),
                    Description ="Activity 8 months in future",
                    Category = "File",
                    City ="Lisbona",
                    Venue="River Thales, Portugal",
                    Latitude = 51.5575525,
                    Longitude = -0.781404
                }
            };

            context.Activites.AddRange(activities);

            await context.SaveChangesAsync();

        }
    }
}
