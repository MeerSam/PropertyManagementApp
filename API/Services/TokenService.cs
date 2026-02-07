using System;
using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config, AppDbContext context) : ITokenService
{
    public async void CleanupExpiredSelectionTokensAsync(string userId)
    {
        try
        {
            var cutoffDate = DateTime.UtcNow.AddHours(-1); // Keep tokens for 1 hour for audit
            
            var expiredTokens = await context.ClientSelectionTokens
                .Where(t => t.UserId == userId && t.ExpiresAt < cutoffDate)
                .ToListAsync();

            if (expiredTokens.Count != 0)
            {
                context.ClientSelectionTokens.RemoveRange(expiredTokens);
                await context.SaveChangesAsync();
                
                 ;
            }
        }
        catch (Exception  )
        {
             
        }
    }

    /// <summary> 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="clientId"></param>
    /// <param name="memberId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> GenerateAccessToken(AppUser user, string clientId)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get token key");
        // if this returns after the ?? 
        if (tokenKey.Length <= 64)
        {
            throw new Exception("The token key length to be >= 64 characters");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        // key is used for signing the token
        // SymmetricSecurityKey used for encrypt and decrypt info

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new ("ClientId", clientId ?? "")
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds

        }; // changed from AddMinutes(7) in order to keep the JWT longer 

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    ///
    /// 
    {
        // store this refresh token which is going to be a long live token 
        // along with our user objects in the database and 
        // return it to the client browser as a cookie, an HTTP only secure cookie.

        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);

    }

    public string GenerateSelectClientToken(AppUser user)
    {
        var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot get token key");
        // if this returns after the ?? 
        if (tokenKey.Length <= 64)
        {
            throw new Exception("The token key length to be >= 64 characters");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        // key is used for signing the token
        // SymmetricSecurityKey used for encrypt and decrypt info 

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.NameIdentifier, user.Id),
            new ("TokenType", "SelectClientToken"),

        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = creds

        }; // changed from AddMinutes(7) in order to keep the JWT longer 

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }


    /*
    public async Task<string>  ValidateSelectionToken(string selectionToken)
    ///Option 2: Signed State Token (Simpler Alternative)
    /// If you don't want to store tokens in the database, you can use a self-contained signed token.
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["TokenKey"]!);

        try
        {
            var principal = tokenHandler.ValidateToken(selectionToken, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            // Verify it's a selection token
            var tokenTypeClaim = principal.FindFirst("token_type");
            if (tokenTypeClaim?.Value != "selection")
            {
                throw new SecurityTokenException("Invalid token type");
            }

            // Get userId from token
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? throw new SecurityTokenException("User ID not found in token");

            return userIdClaim.Value;
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Invalid or expired selection token", ex);
        }
    }

    */


}
