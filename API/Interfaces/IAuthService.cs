using System;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using Microsoft.AspNetCore.Authentication;

namespace API.Interfaces;

public interface IAuthService
{
    Task<ClientSelectLoginResponseDto> AuthenticateAsync(LoginDto loginDto);
    ///Purpose: Validate credentials 
    ///Returns: List of available clients user can access along with selection token (short lived)    
    ///Throws: UnauthorizedAccessException: return AuthErrorResponseDto, 401
    Task<AuthSuccessResponseDto> CompleteLoginAsync(string selectionToken, SelectClientRequestDto request);
    ///Purpose: User selected client, generate JWT with that client context
    ///Returns: JWT token with ClientId and MemberId claims
    ///Throws: UnauthorizedAccessException : return AuthErrorResponseDto, 401 
    ///  if user doesn't have access to selected client 

    Task LogOutAsync();
    Task<AuthSuccessResponseDto> RefreshTokenAsync(string refreshToken);
    ///Purpose: Get new access token without re-login
    ///Note: Refresh maintains same ClientId context
    Task<UserDto> RegisterAsync(RegisterDto registerDto, string clientId);


}
