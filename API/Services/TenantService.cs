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

    public bool HasAccessToClient(string clientId)
    {
        return GetCurrentClientId() == clientId;
    }
}
