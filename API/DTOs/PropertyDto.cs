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
    public string? Country { get; set; } = string.Empty;
    public string? LotNumber { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }  

    public string? AssignedParking { get; set; } = string.Empty;

    public DateTime LastUpdated { get; set; }  
    public string LastUpdatedBy { get; set; } = string.Empty; // DisplayName

    public int? SquareFeet { get; set; }  
    public int? Bedrooms { get; set; } 
    public int? Bathrooms { get; set; }  
    public bool IsRented { get; set; } 
    public bool IsSameAddress { get; set; } = false;
    public string? MailAddress { get; set; } = string.Empty;
    public string? MailUnit { get; set; } = string.Empty;
    public string? MailCity { get; set; } = string.Empty;
    public string? MailState { get; set; } = string.Empty;
    public string? MailZipCode { get; set; } = string.Empty;
    public string? MailCountry { get; set; } = string.Empty;


    // Multi-Tenant field
    public string ClientId { get; set; } = null!; 
    public ICollection<PropertyOwnershipDto> Ownerships { get; set; } = [];
    public ICollection<MemberDto> CurrentOwners { get; internal set; } = [];
}
