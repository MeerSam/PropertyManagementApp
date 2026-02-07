using System;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class AuthService(AppDbContext context, ITokenService tokenService
        , IClientRepository clientRepository
        , IHttpContextAccessor httpContextAccessor) : IAuthService
{
    /// <summary>
    ///     Purpose: Validate credentials, return available clients
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns>ClientSelectLoginResponseDto.List of clients user can access </returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<ClientSelectLoginResponseDto> AuthenticateAsync(LoginDto loginDto)
    {
        // Step 1: Find user by email
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email!.ToLower() == loginDto.Email.ToLower()) ?? throw new UnauthorizedAccessException("Invalid creadentials entered");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        // Step 2: Validate password
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        try
        {
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) throw new UnauthorizedAccessException("Invalid Username or Password");
            }
        }
        catch (System.Exception)
        {
            throw new UnauthorizedAccessException("Error during authetication");
        }
        // Step 3: Check if account is locked out

        // Step 4: Reset access failed count on successful login

        // Step 5: Get all active client access for this user 
        var availableClients = await clientRepository.GetClientsByUserIdAsync(user.Id) ?? throw new UnauthorizedAccessException("You don't have access to any HOA communities");
        var clientAccess = await context.UserClientAccess
            .Where(uca => uca.UserId == user.Id && uca.IsActive)
            .ToListAsync();

        if (clientAccess.Count < 1)
        {
            throw new UnauthorizedAccessException("You don't have access to any HOA communities");
        }
        // Step 6: For each client, check if user has a member profile

        // Step 7: Generate short-lived selection token
        var selectionToken = tokenService.GenerateSelectClientToken(user);
        var tokenExpiry = DateTime.UtcNow.AddMinutes(5); // 5-minute expiry
        var tokenIdentifier = Guid.NewGuid().ToString();

        // Step 8: Store token hash in database
        var ipAddress = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

        var tokenRecord = new ClientSelectionToken
        {
            TokenHash = HashToken(selectionToken),
            UserId = user.Id,
            ExpiresAt = tokenExpiry,
            CreatedFromIpAddress = ipAddress,
            CreatedAt = DateTime.UtcNow,
            TokenIdentifier = tokenIdentifier
        };

        context.ClientSelectionTokens.Add(tokenRecord);
        await context.SaveChangesAsync();

        // Step 9: Clean up old expired tokens for this user (async, don't await)
        _ = Task.Run(() => tokenService.CleanupExpiredSelectionTokensAsync(user.Id));

        // Step 10: Return response
        return new ClientSelectLoginResponseDto
        {
            DisplayName = clientAccess.First().User.DisplayName,
            UserId = clientAccess.First().UserId,
            SelectionToken = selectionToken,
            Email = clientAccess.First().User.Email,
            Message = "",
            AvailableClients = [.. availableClients]
        };



    }
    /// <summary>
    ///     Purpose: User selected client, generate JWT with that client context
    ///     Returns: JWT token with ClientId and MemberId claims
    /// </summary>
    /// <param name="selectionToken"></param>
    /// <param name="request">Includes the selected ClientID</param>
    /// <returns>AuthSuccessResponseDto</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task<AuthSuccessResponseDto> CompleteLoginAsync(string selectionToken, SelectClientRequestDto request)


    ///Throws: UnauthorizedAccessException if user doesn't have access to selected client

    {
        //("Completing login - client selection for ClientId: {ClientId}", request.ClientId);
        // Step 1: Validate selection token
        var tokenHash = HashToken(selectionToken);

        var tokenRecord = await context.ClientSelectionTokens
            .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
        if (tokenRecord == null)
        {
            throw new UnauthorizedAccessException("Selection Token Invalid");
        }

        // Step 2: Check if token is expired
        if (tokenRecord.ExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Selection Token is expired. Please login again.");
        }

        // Step 3: Check if token has already been used (CRITICAL for security)
        if (tokenRecord.IsUsed || tokenRecord.UsedAt != null)
        {
            throw new UnauthorizedAccessException("Selection Token not valid. Please login again.");
            //LogSecurityEventAsync() throw new UnauthorizedAccessException("Selection token has already been used. Possible security issue
        }

        // Step 4: Mark token as used (BEFORE any other operations)
        var currentIp = httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        tokenRecord.IsUsed = true;
        tokenRecord.UsedAt = DateTime.UtcNow;
        tokenRecord.UsedFromIpAddress = currentIp;
        await context.SaveChangesAsync();

        // Step 6: Get user from token
        var userId = tokenRecord.UserId;
        var user = await context.Users.FindAsync(userId) ?? throw new UnauthorizedAccessException("User not found"); ;


        // Step 7: Verify user has access to the selected client
        var clientAccess = await context.UserClientAccess
            .Include(uca => uca.Client)
            .FirstOrDefaultAsync(uca =>
                uca.UserId == userId &&
                uca.ClientId == request.ClientId && uca.IsActive);

        if (clientAccess == null)
        {
            throw new UnauthorizedAccessException("You don't have access to this HOA community");
        }

        // Step 8: Check if client is active
        if (!clientAccess.Client.IsActive)
        {
            throw new UnauthorizedAccessException("This HOA community is currently inactive");
        }
        // Step 9: Get member profile for this client (if exists)
        var member = await context.Members
            .FirstOrDefaultAsync(m =>
                m.UserId == userId &&
                m.ClientId == request.ClientId);


        // Step 10: Generate FULL JWT with ClientId and MemberId claims
        var accessToken = await tokenService.GenerateAccessToken(user, request.ClientId);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Step 11: Save refresh token
        user.RefreshToken = HashToken(refreshToken);
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await context.SaveChangesAsync();

        // Step 12: Build active client info
        var activeClient = new ActiveClientUserDto
        {
            ClientId = clientAccess.ClientId,
            ClientName = clientAccess.Client.Name,
            Role = clientAccess.Role,
            MemberId = member?.Id,
            UserId = user.Id
        };


        // Step 13: Get all available clients for future switching
        var allClientAccess = await context.UserClientAccess
            .Include(uca => uca.Client)
            .Include(uca => uca.User)
            .Where(uca => uca.UserId == userId && uca.IsActive)
            .Select(UserClientExtensions.ToDtoProjection())
            .ToListAsync();


        // Step 14: Build user DTO   
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ImageUrl = user.ImageUrl,
            ActiveClient = activeClient,
            AvailableClients = [.. allClientAccess]
        };

        // Step 15: Return authentication response
        return new AuthSuccessResponseDto
        {
            Success = true,
            AccessToken = await tokenService.GenerateAccessToken(user, request.ClientId),
            RefreshToken = tokenService.GenerateRefreshToken(),
            DisplayName = user.DisplayName,
            ClientName = activeClient.ClientName,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = userDto
        };

    }

    public Task LogOutAsync()
    {
        throw new NotImplementedException();
    }

    public Task<AuthSuccessResponseDto> RefreshTokenAsync(string refreshToken)
    ///Purpose: Get new access token without re-login
    ///Note: Refresh maintains same ClientId context
    {
        throw new NotImplementedException();
    }

    public Task<UserDto> RegisterAsync(RegisterDto registerDto)
    {
        throw new NotImplementedException();
    }

    
    private string HashToken(string token)
    {
        // using var hmac = new HMACSHA512(key); // Need a consistent key to produce same hash 
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}
