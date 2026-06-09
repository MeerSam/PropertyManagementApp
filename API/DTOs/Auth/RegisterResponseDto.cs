using System;

namespace API.DTOs.Auth;

public class RegisterResponseDto
{
    public bool Success { get; init; } = false;
    public string  Message { get; init; } = string.Empty;
    public string? Id { get; set; }  
    public string? DisplayName { get; set; } 
    public string? AppRole { get; set; }  //  System Users Role.Identity="AppUser"
    public string? ClientName { get; init;}   
    public string? Role { get; set; } // ClientAccess.Role : this is what determines the role based permissions

    public ActiveClientUserDto? ActiveClient { get; init; } 

}
