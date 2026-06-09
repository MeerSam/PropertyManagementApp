
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class AppUser : IdentityUser
{

    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string Gender { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime InactiveDate { get; set; }
    public string? InactiveById { get; set; }

    // 🔥 Audit fields
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public string LastUpdatedById { get; set; } = null!;
    [JsonIgnore]
    public AppUser? LastUpdatedBy { get; set; } = null!;



    // public required byte[] PasswordHash { get; set; }// implemented now to DOTNET_Identity
    // public required byte[] PasswordSalt { get; set; }// implemented now to DOTNET_Identity

    // public string Id { get; set; } = Guid.NewGuid().ToString(); // implemented now to DOTNET_Identity
    // public required string Email { get; set; }// implemented now to DOTNET_Identity
    public ICollection<Member> Members { get; set; } = [];

    // Navigation to client access (junction table)
    public ICollection<UserClientAccess> ClientAccess { get; set; } = [];

}
