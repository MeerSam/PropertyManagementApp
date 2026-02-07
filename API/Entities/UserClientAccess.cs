using System;
using System.Text.Json.Serialization;

namespace API.Entities;


/// <summary>
/// Junction table: Which AppUsers can access which Clients and in what role
/// This is separate from Member - a user can have access without a Member profile (e.g., CAM)
/// </summary>
public class UserClientAccess
{
    public int Id { get; set; }
    
    // Foreign keys
    public required string UserId { get; set; }
    [JsonIgnore]
    public AppUser User { get; set; } = null!;
    
    public required string ClientId { get; set; }
    [JsonIgnore]
    public Client Client { get; set; } = null!;
    
    // Access details
    public required string Role { get; set; } // "Admin", "BoardMember", "CAM", "Owner", "Resident"
    public bool IsActive { get; set; } = true;
    
    // Audit trail
    public DateTime GrantedDate { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedDate { get; set; }
    public string? GrantedByUserId { get; set; }
    public string? Notes { get; set; }
}