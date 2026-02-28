using System;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    Task CleanupExpiredSelectionTokensAsync(string id);

    // Service for Issuing tokens
    string GenerateAccessToken(AppUser user, string clientId);  
    string GenerateRefreshToken();
    string GenerateSelectClientToken(AppUser user); 
}
