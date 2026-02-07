using System;

namespace API.DTOs;

public class SeedDto
{
    public required string ClientId { get; set; } 
    public required string MemberId { get; set; } 

    public required string UserId { get; set; } 

    public required string Email { get; set; } 

    public required string FirstName { get; set; } 

    public required string LastName { get; set; } 

    public required string Gender { get; set; } 

    public DateOnly DateOfBirth { get; set; } 

    public required string DisplayName { get; set; } 

    public   DateTime Created { get; set; } 
    public   DateTime LastActive { get; set; } 
    public string? Description { get; set; } 
    public required string City { get; set; } 
    public required string Country { get; set; } 
    public   string? ImageUrl { get; set; } 

    public string? Role { get; set; } // "Admin", "BoardMember", "CAM", "Owner", "Resident"


}


public class SeedClientDto
{
    public required string ClientId { get; set; } 
    public required string Name { get; set; } 
    public required string Address { get; set; } 

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

}
