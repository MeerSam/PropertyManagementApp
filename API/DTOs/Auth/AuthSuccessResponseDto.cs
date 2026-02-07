using System;

namespace API.DTOs.Auth;
/// <summary>
/// /// Step 1 : If One Client - 1 User then return JWT with ClientId claims (and MemberId if MemberId exists)
/// /// Step 2: User selects a client - Returns JWT with ClientId and MemberId claims
/// </summary>
public class AuthSuccessResponseDto
{
    public bool Success { get; init; }
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; } 
    public required string ClientName { get; init;}  
    public required string DisplayName { get; init; } 
    public required DateTime ExpiresAt { get; init; }
    public required UserDto User { get; init; }
}
