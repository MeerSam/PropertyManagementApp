using System; 
using System.Text.Json.Serialization;

namespace API.Entities;

public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }

    public string? PublicId { get; set; }

    public bool IsApproved { get; set; } 

    // Navigation Property 1member - can have many photos
    // Nav prop we dont make it required and we assign intial value = null!
    [JsonIgnore]
    public Member Member { get; set; } = null!;

    public string MemberId { get; set; } = null!;
    
}
