using System;

namespace API.DTOs;

public class MemberClientDto
{
    
    public required string MemberId { get; set; }
    public required string UserId { get; set; }
    public required string ClientId { get; set; }

    public required string ClientName { get; set; }
    public required string DisplayName { get; set; }

    public required string Email { get; set; }

    public string? ImageUrl { get; set; }  

}
