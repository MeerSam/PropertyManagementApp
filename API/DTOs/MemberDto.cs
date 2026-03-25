using System;

namespace API.DTOs;

public class MemberDto
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }
    public   string? Gender { get; set; }

    public string? ImageUrl { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }

    // Multi-tenant
    public required string ClientId { get; set; } 

    // Relationship to AppUser (1:N)
    // Nav prop we dont make it required and we assign intial value = null!
    // nullable — member may not have a portal login
    public string? UserId { get; set; } 
     
    public ICollection<PropertyOwnershipDto> PropertyOwnerships { get; set; } = []; // member can have many property ownerships

}
