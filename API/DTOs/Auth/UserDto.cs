using System;
using API.DTOs.Auth;
using API.Entities;

namespace API.DTOs;

public class UserDto
{
    public required string Id { get; set; } 
    public required string Email { get; set; }
    public required string DisplayName { get; set; } 
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }  

    public ActiveClientUserDto? ActiveClient { get; init; }
    public ICollection<UserClientAccessInfoDto>? AvailableClients { get; init; }
    
     
}
