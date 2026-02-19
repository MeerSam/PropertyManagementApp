using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); //Member.Id ≠ AppUser.Id (Member becomes tenant-scoped)

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public required string Gender { get; set; }

    public string? ImageUrl { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }

    // Multi-tenant
    public required string ClientId { get; set; }
    [JsonIgnore]
    public Client Client { get; set; } = null!;

    // Relationship to AppUser (1:N)
    // Nav prop we dont make it required and we assign intial value = null!
    public required string UserId { get; set; }
    [JsonIgnore] 
    public AppUser User { get; set; } = null!;


}
