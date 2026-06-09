using System;

namespace API.DTOs.Auth;

public class SelectClientRequestDto
{
    // public required string UserId { get; set;  }
    // Remove UserId - we'll get it from the selection token
    public required string ClientId { get; set; }

}
