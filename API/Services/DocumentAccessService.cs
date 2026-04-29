using System;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class DocumentAccessService(AppDbContext context,
    IUserClientRepository ucaccess,
    ITenantService tenantService) : IDocumentAccessService
{
    public async Task<bool> CanUploadAsync(string userId, string clientId,
        DocumentScope scope, string? propertyId, string? propertyOwnershipId)
    {
        // Step 1 — user must have active access to this client
        var access = await ucaccess.GetAccessByIdAsync(tenantService.GetCurrentClientId(), userId);
        if (access == null) return false;


        // Only these roles can upload anything
        var uploaderRoles = new[] { "admin", "board_member", "property_manager" };

        if (!uploaderRoles.Contains(access.Role) && (scope == DocumentScope.Community || scope == DocumentScope.Public)) return false;

        // Property or tenure docs require a valid PropertyId
        if (scope != DocumentScope.Community && string.IsNullOrEmpty(propertyId))
            return false;

        // Tenure docs additionally require an OwnershipId
        if (scope == DocumentScope.OwnerTenure &&
            string.IsNullOrEmpty(propertyOwnershipId))
            return false;


        return true;
    }

    public async Task<bool> CanViewAsync(string userId, Document document)
    {
        // Step 1 — user must have active access to this client
        var access = await ucaccess.GetAccessByIdAsync(tenantService.GetCurrentClientId(), userId);
        if (access == null) return false;

        //2.  Make sure user belong to the same client
        if (access.ClientId != document.ClientId) return false;

        return document.Scope switch
        {
            DocumentScope.Public => true, // anyone with link
            DocumentScope.Community => true, // any active role for this client
            DocumentScope.PropertyHistory => await CanViewPropertyHistory(access, userId, document),
            DocumentScope.OwnerTenure => await CanViewOwnerTenure(access, userId, document),
            _ => false
        };

    }

    private async Task<bool> CanViewPropertyHistory(UserClientAccess access, string userId, Document document)
    {
        // Board, admin/ Managers see all docs
        if (access.Role is "admin" or "property_manager") return true;
        // property docs like previous maintence records and arc forms 

        // Board members can view property history docs
        if (access.Role == "board_member") return true;

        // Owner — must be current Primary owner of this property
        if (access.Role == "owner")
            return await IsPrimaryOwner(userId,
                document.ClientId,
                document.PropertyId!,
                requireCurrent: true);

        return false;

    }

    private async Task<bool> IsPrimaryOwner(string userId, string clientId, string propertyId, bool requireCurrent)
    {
        var member = await context.Members
            .FirstOrDefaultAsync(m => m.UserId == userId
                && m.ClientId == clientId);

        if (member == null) return false;

        var query = context.PropertyOwnerships.Where(po =>
            po.PropertyId == propertyId &&
            po.MemberId == member.Id &&
            po.OwnershipType == OwnershipType.Primary);

        if (requireCurrent)
            query = query.Where(po => po.IsCurrent && po.EndDate == null);

        return await query.AnyAsync();

    }

    private async Task<bool> CanViewOwnerTenure(UserClientAccess access, string userId, Document document)
    {
        // Board, admin/ Managers see all docs
        if (access.Role is "admin" or "property_manager") return true;

        // Board members cannot see tenure docs (personal ownership records) for other residents.
        var member = await context.Members
                .FirstAsync(m => m.UserId == userId && m.ClientId == document.ClientId);

        if (member is null) return false;


        // Owner/board_member could be an owner — their Member must match the specific PropertyOwnership record
        // Note: we do NOT check IsCurrent here — tenure docs belong
        // to that ownership record permanently
        if (access.Role == "owner" && access.Role == "board_member")
        {
            return await context.PropertyOwnerships.AnyAsync(po =>
                po.Id == document.PropertyOwnershipId &&
                po.MemberId == member.Id &&
                po.OwnershipType == OwnershipType.Primary);
        }
        return false;
    }
}
