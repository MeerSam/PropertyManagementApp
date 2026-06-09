using System;
using API.DTOs.Auth;
using API.Entities;

namespace API.DTOs.Auth;

public class UserDto
{
    public required string Id { get; set; } 
    public required string Email { get; set; }
    public required string DisplayName { get; set; } 
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }  



    public string? AppRole { get; set; }  // System Role all get app user role except Super Admin
    public string? Role { get; set; }  // Client Access per client role this is what determines the role based permissions


    public ActiveClientUserDto? ActiveClient { get; init; }
    public ICollection<UserClientAccessInfoDto>? AvailableClients { get; init; }
    
     
}
