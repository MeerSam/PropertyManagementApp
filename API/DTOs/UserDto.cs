using System;

namespace API.DTOs;

public class UserDto
{
    public required string Id { get; set; }
    public required string ClientId { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public string? ImageUrl { get; set; }  
    
    public required string Token { get; set; } // TokenServices Creates a Token and returns it
}
