using System;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Client
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    // NO direct Users collection - users are accessed via Members
    [JsonIgnore]
    public ICollection<UserClientAccess> UsersAccess { get; set; } = [];
    [JsonIgnore]
    public ICollection<Property> Properties { get; set; } = [];
    [JsonIgnore]
    public ICollection<Member> Members { get; set; } = [];
}

