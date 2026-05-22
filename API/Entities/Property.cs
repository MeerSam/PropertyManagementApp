using System;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Property
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Address { get; set; }
    public string? Unit { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; } 
    public string? Country { get; set; }  

    public string? LotNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? SquareFeet { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public bool IsRented { get; set; } = false;

    public string? AssignedParking { get; set; }  

    // public bool IsSameMailAddress { get; set; } = false;
    public bool IsSameAddress { get; set; } = false;
    public string? MailAddress { get; set; } 
    public string? MailUnit { get; set; } 
    public string? MailCity { get; set; } 
    public string? MailState { get; set; } 
    public string? MailZipCode { get; set; } 
    public string? MailCountry { get; set; }  

    // 🔥 Audit fields
    public DateTime LastUpdated { get; set; }
    public string LastUpdatedById { get; set; } = null!;
     [JsonIgnore]
    public AppUser? LastUpdatedBy { get; set; } = null!;


    // Multi-Tenant field
    public string ClientId { get; set; } = null!;
    [JsonIgnore]
    public Client Client { get; set; } = null!;

    public ICollection<PropertyOwnership> Ownerships { get; set; } = []; 
 
    // // Convenience
    // public IEnumerable<PropertyOwnership> CurrentOwners =>
    //     Ownerships.Where(o => o.IsCurrent);

    // public PropertyOwnership? PrimaryOwner =>
    //     Ownerships.FirstOrDefault(o => o.IsCurrent && o.OwnershipType == OwnershipType.Primary);
}
