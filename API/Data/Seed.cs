using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

    public static async Task SeedData(AppDbContext context)
    {
        // Already seeded

        var rawData = await File.ReadAllTextAsync("Data/SeedData.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = JsonSerializer.Deserialize<SeedDataRoot>(rawData, options);
        if (data == null) return;

        // 1. Clients (Tenant root)
        if (!await context.Clients.AnyAsync())
        {
            context.Clients.AddRange(data.Clients);
            await context.SaveChangesAsync();
        }


        // 2. AppUsers (Global identities)
        if (!await context.Users.AnyAsync())
        {
            using var hmac = new HMACSHA512(); // removed since using ASPNET IDENTITY
            foreach (var user in data.Users)
            {
                var newuser = new AppUser
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = user.Gender,
                    PasswordSalt = hmac.Key,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                    Created = user.Created,
                    DateOfBirth = user.DateOfBirth,
                    ImageUrl = user.ImageUrl
                };
                context.Users.Add(newuser);
            }
            var result = await context.SaveChangesAsync();
            if (result < 0) return;

        }

        // 3. Members (Client-scoped people)
        if (!await context.Members.AnyAsync())
        {
            context.Members.AddRange(data.Members);
            await context.SaveChangesAsync();
        }


        // 4. Properties (Tenant-scoped)
        if (!await context.Properties.AnyAsync())
        {
            context.Properties.AddRange(data.Properties);
            await context.SaveChangesAsync();
        }


        // 5. UserClientAccess (junction: AppUser ↔ Client)
        if (!await context.UserClientAccess.AnyAsync())
        {
            context.UserClientAccess.AddRange(data.UserClientAccess);
            await context.SaveChangesAsync();
        }


        // 6. PropertyOwnership (historical)
        if (!await context.PropertyOwnerships.AnyAsync())
        {
            var count = 0;
            foreach (var ownership in data.PropertyOwnerships)
            {
                if (count < 25)
                {
                    count++;
                    var owner_record = new PropertyOwnership
                    {
                        Id = ownership.Id,
                        PropertyId = ownership.PropertyId,
                        MemberId = ownership.MemberId,
                        OwnershipType = ownership.OwnershipType,
                        IsCurrent = ownership.IsCurrent,
                        StartDate = ownership.StartDate,
                        EndDate = ownership.EndDate,
                        OwnershipPercentage = ownership.OwnershipPercentage

                    };
                    context.PropertyOwnerships.Add(owner_record);
                }

            }
            var result = await context.SaveChangesAsync();
            if (result < 0) return;

        }

    }
}


