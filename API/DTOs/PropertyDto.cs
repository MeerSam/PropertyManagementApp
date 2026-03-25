using System;
using API.Entities;

namespace API.DTOs;

public class PropertyDto
{
    public string? Id { get; set; } =null;
    public required string Address { get; set; } = string.Empty;
    public string? Unit { get; set; } = string.Empty;
    public string? City { get; set; } = string.Empty;
    public string? State { get; set; } = string.Empty;
    public string? ZipCode { get; set; } = string.Empty;
    public string? LotNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }  

    public int? SquareFeet { get; set; }  
    public int? Bedrooms { get; set; } 
    public int? Bathrooms { get; set; }  
    public bool IsRented { get; set; } 


    // Multi-Tenant field
    public string ClientId { get; set; } = null!; 
    public ICollection<PropertyOwnershipDto> Ownerships { get; set; } = [];
    public ICollection<MemberDto> CurrentOwners { get; internal set; } = [];
}
