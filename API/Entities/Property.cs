using System;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Property
{
    public int Id { get; set; }
    public required string Address { get; set; }
    public string? Unit { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? LotNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? SquareFeet { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public bool IsRented { get; set; } = false;


    // Multi-Tenant field
    public string ClientId { get; set; }= null!;
    [JsonIgnore]
    public Client Client { get; set; } = null!;
 

}
