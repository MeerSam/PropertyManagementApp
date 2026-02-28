using System;

namespace API.DTOs;

public class MemberClientDto
{
    
    public required string MemberId { get; set; }
    public   string? UserId { get; set; } =null; // User may be null not all login users have member profile and vice-versa
    public required string ClientId { get; set; }

    public required string ClientName { get; set; }
    public required string DisplayName { get; set; }

    public required string Email { get; set; }

    public string? ImageUrl { get; set; }  

}
