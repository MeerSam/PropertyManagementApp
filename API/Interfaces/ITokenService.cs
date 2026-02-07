using System;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    void CleanupExpiredSelectionTokensAsync(string id);

    // Service for Issuing tokens
    Task<string> GenerateAccessToken(AppUser user, string clientId);  
    string GenerateRefreshToken();
    string GenerateSelectClientToken(AppUser user); 
}
