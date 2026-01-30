using System;

namespace API.Entities;

public class Client
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
     
    public ICollection<Property> Properties { get; set; } = [];
    public ICollection<Member> Members { get; set; } = [];
}

