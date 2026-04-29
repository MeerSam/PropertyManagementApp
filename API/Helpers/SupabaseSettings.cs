using System;

namespace API.Helpers;

public class SupabaseSettings
{
    public required string Url { get; set; }
    public required string ServiceRoleKey { get; set; }
    public required string BucketName { get; set; }
    
    public int PresignedUrlExpiryMinutes { get; set; } = 15;

}
