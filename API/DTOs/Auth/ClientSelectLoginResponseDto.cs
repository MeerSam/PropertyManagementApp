using System;

namespace API.DTOs;

public class ClientSelectLoginResponseDto
{
    /// <summary>
    /// The DTO contains details to be sent after Login Step 1: Successfull
    /// Returns the Active List of Clients and short lived SelectionToken
    /// </summary>
    public required string UserId { get; set; }
    public required string DisplayName { get; init; }
    public required string Email { get; init; }
    public required string Message { get; init; } = "Please select which HOA you want to access";
    public required string SelectionToken { get; init; } // NEW: Special token for step 2
    public required ICollection<UserClientAccessInfoDto>? AvailableClients { get; set; }
}
