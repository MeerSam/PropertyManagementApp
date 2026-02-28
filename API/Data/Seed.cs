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
            context.PropertyOwnerships.AddRange(data.PropertyOwnerships);
            await context.SaveChangesAsync();

        }

    }
    public static async Task SeedDataOld(AppDbContext context)
    {
        if (!await context.Clients.AnyAsync())
        {

            var rawClientData = await File.ReadAllTextAsync("Data/ClientSeedData.json");

            var clients = JsonSerializer.Deserialize<List<SeedClientDto>>(rawClientData);

            if (clients != null)
            {
                foreach (var clientInfo in clients)
                {

                    var client = new Client
                    {
                        Id = clientInfo.ClientId,
                        Name = clientInfo.Name,
                        Address = clientInfo.Address,
                        IsActive = clientInfo.IsActive
                    };
                    context.Clients.Add(client);

                }
                ;
            }

            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var rawMemberData = await File.ReadAllTextAsync("Data/MemberSeedData.json");

            var members = JsonSerializer.Deserialize<List<SeedDto>>(rawMemberData);

            if (members != null)
            {
                // we need to work on password Hash
                using var hmac = new HMACSHA512(); // removed since using ASPNET IDENTITY

                foreach (var member in members)
                {
                    var existUser = await context.Users.FirstOrDefaultAsync(x => x.Id == member.UserId);
                    var client = await context.Clients.FirstAsync(x => x.Id == member.ClientId);
                    if (existUser == null)
                    {
                        var user = new AppUser
                        {
                            Id = member.UserId,
                            Email = member.Email,
                            // UserName = member.Email, // must be in for AspNET Identity
                            DisplayName = member.DisplayName,
                            ImageUrl = member.ImageUrl,
                            FirstName = member.FirstName,
                            LastName = member.LastName,
                            Gender = member.Gender,
                            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd")),
                            PasswordSalt = hmac.Key
                        };

                        if (client != null)
                        {
                            user.ClientAccess.Add(new UserClientAccess
                            {
                                UserId = user.Id,
                                ClientId = client.Id,
                                Role = member.Role ?? "Owner",
                                IsActive = true,
                                GrantedDate = DateTime.Now,
                            });


                            user.Members.Add(new Member
                            {
                                Id = member.MemberId,
                                ImageUrl = member.ImageUrl,
                                Email = member.Email,
                                DisplayName = member.DisplayName,
                                FirstName = member.FirstName,
                                LastName = member.LastName,
                                Gender = member.Gender,
                                ClientId = client.Id,
                                UserId = user.Id
                            });
                        }
                        if (!await context.Users.AnyAsync(u => u.Id == member.UserId))
                        {
                            context.Users.Add(user);
                        }
                    }
                    else
                    {
                        var access = await context.UserClientAccess.FirstOrDefaultAsync(x => x.ClientId == member.ClientId && x.UserId == member.UserId);
                        if (access == null)
                        {
                            existUser.ClientAccess.Add(new UserClientAccess
                            {
                                UserId = existUser.Id,
                                ClientId = client.Id,
                                Role = member.Role ?? "Owner",
                                IsActive = true,
                                GrantedDate = DateTime.Now,
                            });
                        }

                        if (!await context.Members.AnyAsync(x => x.Id == member.MemberId))
                        {
                            existUser.Members.Add(new Member
                            {
                                Id = member.MemberId,
                                ImageUrl = member.ImageUrl,
                                Email = member.Email,
                                DisplayName = member.DisplayName,
                                FirstName = member.FirstName,
                                LastName = member.LastName,
                                Gender = member.Gender,
                                ClientId = client.Id,
                                UserId = existUser.Id
                            });
                        }
                    }
                }

                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
            }

        }
    }
}


