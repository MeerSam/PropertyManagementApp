using System;
using API.Entities;

namespace API.DTOs;

public class PropertyOwnershipDto
{
public int? Id { get; set; } 
    public string? PropertyId { get; set; }  
    public string? MemberId { get; set; } 

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }  // null = still active

    public string? OwnershipType { get; set; }  // Primary, CoOwner
    public decimal? OwnershipPercentage { get; set; } // optional — e.g. 50/50 split
    public bool IsCurrent { get; set; }

}
