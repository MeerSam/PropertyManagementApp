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
                    var existUser =  await context.Users.FirstOrDefaultAsync(x => x.Id == member.UserId);
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
                        var access  = await  context.UserClientAccess.FirstOrDefaultAsync(x => x.ClientId == member.ClientId && x.UserId == member.UserId);
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

                        if(! await context.Members.AnyAsync(x => x.Id == member.MemberId))
                        {
                            existUser.Members.Add(new Member
                            {
                                Id = member.MemberId,
                                ImageUrl = member.ImageUrl,
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
