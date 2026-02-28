using System;
using System.Text.Json.Serialization;

namespace API.Entities;

public class PropertyOwnership
{
    public int Id { get; set; }
    public required string PropertyId { get; set; }  
    public required string MemberId { get; set; } 

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }  // null = still active

    public OwnershipType OwnershipType { get; set; }  // Primary, CoOwner
    public decimal? OwnershipPercentage { get; set; } // optional — e.g. 50/50 split
    public bool IsCurrent { get; set; }


    // Navigation
     [JsonIgnore]
    public Property Property { get; set; } = null!;
    [JsonIgnore]
    public Member Member { get; set; } = null!;
}
 
public enum OwnershipType
{
    Primary,
    CoOwner
}
