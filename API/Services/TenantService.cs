using System;
using System.Security.Claims;
using API.Interfaces;

namespace API.Services;

public class TenantService(IHttpContextAccessor httpContextAccessor) : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string GetCurrentClientId()
    {
        // throw new NotImplementedException();
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new UnauthorizedAccessException("No active HTTP context");
        }

        var clientIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("ClientId");

        if (clientIdClaim == null || string.IsNullOrWhiteSpace(clientIdClaim.Value))
        {
            throw new UnauthorizedAccessException("Client ID not found in token");
        }

        return clientIdClaim.Value;


    }

    public string GetCurrentMemberId()
    {
       // throw new NotImplementedException();
        if (_httpContextAccessor.HttpContext == null)
        {
            throw new UnauthorizedAccessException("No active HTTP context");
        }

        var memberIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("MemberId");

        if (memberIdClaim == null || string.IsNullOrWhiteSpace(memberIdClaim.Value))
        {
            throw new UnauthorizedAccessException("Client ID not found in token");
        }

        return memberIdClaim.Value;

    }

    public string GetCurrentUserId()
    {
        var userIdClaim = (_httpContextAccessor.HttpContext?.User
            .FindFirst(ClaimTypes.NameIdentifier)) ?? throw new UnauthorizedAccessException("User ID not found in token");
        return userIdClaim.Value;
    }

    public bool HasAccessToClient(string clientId)
    {
        return GetCurrentClientId() == clientId;
    }

    
}
