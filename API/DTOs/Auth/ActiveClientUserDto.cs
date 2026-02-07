using System;
using API.Entities;

namespace API.DTOs.Auth;

public class ActiveClientUserDto
{
    public required string ClientId { get; init; }
    public required string ClientName { get; init; }

    public required string UserId { get; set; }
    public string? MemberId { get; set; }

    public required string Role { get; set; }
    

}
