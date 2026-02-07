using System;

namespace API.DTOs;

public class UserClientAccessInfoDto
{
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
    public required string Email { get; set; }


    public required string ClientId { get; set; }

    public required string ClientName { get; set; }


    public   string? MemberId { get; set; }

    public bool HasMemberProfile { get; init; }

    public string? ImageUrl { get; set; }  

}
