using System;
using API.Data;
using API.Entities;
using API.Helpers;
using API.Interfaces;

namespace API.Services;


public sealed class AuthorizationService(IPropertyRepository propertyRepository) : IAuthorizationService
{
    public   Task<bool> CanCreatePropertyAsync(UserAccess access)
    {
        if (access == null) return  Task.FromResult(false);

        if (HoaRoles.priviledgedRoles.Contains(access.Role)) return  Task.FromResult(true);

        return  Task.FromResult(false);
    }

    public   Task<bool> CanDeletePropertyAsync(UserAccess access, string propertyId)
    {
        if (access == null) return  Task.FromResult(false);

        if (HoaRoles.priviledgedRoles.Contains(access.Role)) return  Task.FromResult(true);

        return  Task.FromResult(false);
    }

    public Task<bool> CanUpdatePropertyAsync(UserAccess access, string propertyId)
    {
        if (access == null) return  Task.FromResult(false);

        if (HoaRoles.priviledgedRoles.Contains(access.Role)) return  Task.FromResult(true);

        return  Task.FromResult(false);
    }

    public Task<bool> CanUploadDocumentAsync(string userId, string clientId, DocumentScope scope, string? propertyId, string? propertyOwnershipId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanViewDocumentAsync(string userId, Document document)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CanViewLimitedPropertyAsync(UserAccess access, string propertyId)
    {
        if (access == null) return Task.FromResult(false);

        if (HoaRoles.priviledgedRoles.Contains(access.Role)) return  Task.FromResult(true);

        return  Task.FromResult(false);
    }

    public async Task<bool> CanViewPropertyAsync(UserAccess access, string propertyId)
    {
        if (access == null) return false;
        // Board, Admin and Property Managers can see all 
        if (HoaRoles.priviledgedRoles.Contains(access.Role)) return true;

        var property = await propertyRepository.GetPropertyAsync(propertyId);

        return false;

    }
}