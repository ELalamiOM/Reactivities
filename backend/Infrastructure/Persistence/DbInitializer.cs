using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
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
            if (context.Activities.Any()) return;

            var members = await userManager.Users.ToListAsync();

           var activities = new List<Activity>
{
    new()
    {
        Title = "Festival de Jazz de Casablanca",
        Date = DateTime.UtcNow.AddDays(5),
        Description = "Une soirée musicale avec plusieurs artistes de jazz marocains et internationaux.",
        Category = "Music",
        City = "Casablanca",
        Venue = "Anfa Park",
        Latitude = 33.5731,
        Longitude = -7.6415,
        Attendees =
        [
            new() { User = members[0], IsHost = true },
            new() { User = members[1], IsHost = false }
        ]
    },
    new()
    {
        Title = "Découverte de Chefchaouen",
        Date = DateTime.UtcNow.AddDays(10),
        Description = "Excursion d'une journée dans la célèbre ville bleue du Maroc.",
        Category = "Travel",
        City = "Chefchaouen",
        Venue = "Place Outa El Hammam",
        Latitude = 35.1714,
        Longitude = -5.2697,
        Attendees =
        [
            new() { User = members[1], IsHost = true },
            new() { User = members[0], IsHost = false }
        ]
    },
    new()
    {
        Title = "Tournoi de Football Entreprises",
        Date = DateTime.UtcNow.AddDays(15),
        Description = "Compétition amicale entre équipes de différentes sociétés.",
        Category = "Sports",
        City = "Casablanca",
        Venue = "Stade Mohammed V",
        Latitude = 33.5792,
        Longitude = -7.6426,
        Attendees =
        [
            new() { User = members[0], IsHost = true },
            new() { User = members[2], IsHost = false }
        ]
    },
    new()
    {
        Title = "Atelier Développement .NET",
        Date = DateTime.UtcNow.AddDays(8),
        Description = "Formation sur ASP.NET Core, Clean Architecture et MediatR.",
        Category = "Technology",
        City = "Rabat",
        Venue = "Technopolis",
        Latitude = 33.9822,
        Longitude = -6.8651,
        Attendees =
        [
            new() { User = members[1], IsHost = true }
        ]
    },
    new()
    {
        Title = "Salon de l'Entrepreneuriat",
        Date = DateTime.UtcNow.AddDays(20),
        Description = "Rencontre entre startups, investisseurs et porteurs de projets.",
        Category = "Business",
        City = "Rabat",
        Venue = "Centre International de Conférences",
        Latitude = 34.0209,
        Longitude = -6.8416,
        Attendees =
        [
            new() { User = members[2], IsHost = true },
            new() { User = members[0], IsHost = false }
        ]
    },
    new()
    {
        Title = "Marathon International de Marrakech",
        Date = DateTime.UtcNow.AddDays(25),
        Description = "Course annuelle dans les rues emblématiques de Marrakech.",
        Category = "Sports",
        City = "Marrakech",
        Venue = "Avenue Mohammed VI",
        Latitude = 31.6295,
        Longitude = -7.9811,
        Attendees =
        [
            new() { User = members[0], IsHost = true },
            new() { User = members[1], IsHost = false }
        ]
    },
    new()
    {
        Title = "Dégustation de Cuisine Marocaine",
        Date = DateTime.UtcNow.AddDays(12),
        Description = "Découverte des saveurs traditionnelles marocaines.",
        Category = "Food",
        City = "Fès",
        Venue = "Riad Rcif",
        Latitude = 34.0331,
        Longitude = -5.0003,
        Attendees =
        [
            new() { User = members[1], IsHost = true },
            new() { User = members[2], IsHost = false }
        ]
    },
    new()
    {
        Title = "Exposition d'Art Contemporain",
        Date = DateTime.UtcNow.AddDays(18),
        Description = "Présentation d'œuvres d'artistes marocains contemporains.",
        Category = "Culture",
        City = "Casablanca",
        Venue = "Villa des Arts",
        Latitude = 33.5928,
        Longitude = -7.6192,
        Attendees =
        [
            new() { User = members[2], IsHost = true }
        ]
    },
    new()
    {
        Title = "Hackathon Innovation Digitale",
        Date = DateTime.UtcNow.AddDays(30),
        Description = "48 heures pour développer des solutions innovantes.",
        Category = "Technology",
        City = "Tanger",
        Venue = "Tanger Tech",
        Latitude = 35.7595,
        Longitude = -5.8340,
        Attendees =
        [
            new() { User = members[0], IsHost = true },
            new() { User = members[1], IsHost = false }
        ]
    },
    new()
    {
        Title = "Randonnée au Mont Toubkal",
        Date = DateTime.UtcNow.AddDays(35),
        Description = "Ascension guidée du plus haut sommet d'Afrique du Nord.",
        Category = "Travel",
        City = "Imlil",
        Venue = "Parc National du Toubkal",
        Latitude = 31.0675,
        Longitude = -7.9180,
        Attendees =
        [
            new() { User = members[2], IsHost = true },
            new() { User = members[0], IsHost = false }
        ]
    },
    new()
    {
        Title = "Festival International du Film",
        Date = DateTime.UtcNow.AddDays(40),
        Description = "Projection de films et rencontres avec des réalisateurs.",
        Category = "Film",
        City = "Marrakech",
        Venue = "Palais des Congrès",
        Latitude = 31.6258,
        Longitude = -8.0161,
        Attendees =
        [
            new() { User = members[1], IsHost = true },
            new() { User = members[2], IsHost = false }
        ]
    },
    new()
    {
        Title = "Journée de Nettoyage de la Plage",
        Date = DateTime.UtcNow.AddDays(6),
        Description = "Action citoyenne pour préserver le littoral marocain.",
        Category = "Community",
        City = "Agadir",
        Venue = "Plage d'Agadir",
        Latitude = 30.4278,
        Longitude = -9.5981,
        Attendees =
        [
            new() { User = members[0], IsHost = true },
            new() { User = members[1], IsHost = false },
            new() { User = members[2], IsHost = false }
        ]
    }
};
            context.Activities.AddRange(activities);

            await context.SaveChangesAsync();

        }
    }
}
