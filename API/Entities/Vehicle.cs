using System;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Vehicle
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    // Vehicle Info
    public string VehicleMake { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string VehicleYear { get; set; } = string.Empty;
    public string VehicleColor { get; set; } = string.Empty;
    public string RegisteredState { get; set; } = string.Empty;
    public string CurrentTag { get; set; } = string.Empty;
    public DateOnly DateRegistered { get; set; }
    public string? VIN { get; set; }

    // Required Owner (Member)
    public required string OwnerId { get; set; }
    public Member Owner { get; set; } = null!;

    // Optional AppUser (if the member is also a user)
    public string? UserId { get; set; }
    public AppUser? User { get; set; }

    // Multi-tenant
    public required string ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ApprovedDate { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime? DateRemoved { get; set; }

    public AppUser? RemovedBy { get; set; }


}
