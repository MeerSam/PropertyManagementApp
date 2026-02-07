using System;

namespace API.Entities;

public class ClientSelectionToken
{
    public int Id { get; set; }
    public required string TokenHash { get; set; } // Store hashed token
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public required string UserId { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedAt { get; set; }
    public string? UsedFromIpAddress { get; set; }
    public string? CreatedFromIpAddress { get; set; }
    public string? TokenIdentifier { get; set; }

} 