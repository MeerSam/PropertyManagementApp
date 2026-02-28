using System;
using API.Entities;

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

    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public string? ImageUrl { get; set; }

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

public class UserSeedData
{
    public required string Id { get; set; }
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string Gender { get; set; }
    public   string? UserId { get; set; } 
    public   string? ClientId { get; set; }
    public string? Description { get; set; }

}

public class SeedDataRoot
{
    public List<Client> Clients { get; set; } = new();
    public List<UserSeedData> Users { get; set; } = new();
    public List<Member> Members { get; set; } = new();
    public List<UserClientAccess> UserClientAccess { get; set; } = new();
    public List<Property> Properties { get; set; } = new();
    public List<PropertyOwnership> PropertyOwnerships { get; set; } = new();
}

