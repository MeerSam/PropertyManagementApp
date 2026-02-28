using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); //Member.Id ≠ AppUser.Id (Member becomes tenant-scoped)
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
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
    public   string? UserId { get; set; } // nullable — member may not have a portal login
    [JsonIgnore] 
    public AppUser? User { get; set; } = null!;
    [JsonIgnore] 
    public ICollection<PropertyOwnership> PropertyOwnerships { get; set; } = []; // member can have many property ownerships

   /*  ## Visual Relationship Summary
```
AppUser (IdentityUser)
    │
    └──< UserClientAccess >──── Client
              │                    │
              │                    └──< Member
              │                           │
              └──< RefreshToken    (optional) AppUser
The Member ──► AppUser link is optional — a member can exist as just a record (e.g. a homeowner who hasn't registered),
and an AppUser can exist without any Member profile (e.g. a property manager). */


}
