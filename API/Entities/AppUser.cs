
using System.Text.Json.Serialization;

namespace API.Entities;

public class AppUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string Gender { get; set; }

    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    [JsonIgnore]
    public ICollection<Member> Members { get; set; } = [];

    // Navigation to client access (junction table)
    [JsonIgnore]
    public ICollection<UserClientAccess> ClientAccess { get; set; } = [];

}
