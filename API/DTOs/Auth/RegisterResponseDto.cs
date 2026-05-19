using System;

namespace API.DTOs.Auth;

public class RegisterResponseDto
{
    public bool Success { get; init; } = false;
    public string  Message { get; init; } = string.Empty;
    public string? Id { get; set; }  
    public string? DisplayName { get; set; } 
    public string? AppRole { get; set; } 
    public string? ClientName { get; init;}   
    public ActiveClientUserDto? ActiveClient { get; init; } 

}
