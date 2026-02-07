using System;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using Microsoft.AspNetCore.Authentication;

namespace API.Interfaces;

public interface IAuthService
{
    Task<ClientSelectLoginResponseDto> AuthenticateAsync(LoginDto loginDto);
    ///Purpose: User selected client, generate JWT with that client context
    ///Returns: JWT token with ClientId and MemberId claims
    ///Throws: UnauthorizedAccessException if user doesn't have access to selected client
    Task<AuthSuccessResponseDto> CompleteLoginAsync(string selectionToken , SelectClientRequestDto request);
    ///Purpose: Validate credentials, return available clients
    ///Returns: List of clients user can access
    ///Throws: UnauthorizedAccessException if credentials invalid 

    Task LogOutAsync();
    Task<AuthSuccessResponseDto> RefreshTokenAsync(string refreshToken);
    ///Purpose: Get new access token without re-login
    ///Note: Refresh maintains same ClientId context
    Task<UserDto> RegisterAsync(RegisterDto registerDto);
    

}
