using System;
using API.DTOs.Docs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class DocumentRepository(AppDbContext context,
ITenantService tenantService) : IDocumentRepository
{
    public async Task<IReadOnlyList<DocumentDto>> GetCommunityDocumentsByClient(string clientId)
    {
        //  Mkae sure the access is checked 
        return await context.Documents
            .Where(d => d.ClientId == clientId 
                && d.IsActive == true 
                && (d.Scope == DocumentScope.Community || d.Scope == DocumentScope.Public))
            .OrderByDescending(d=> d.UploadedAt)
            .Select(DocumentExtensions.ToDtoProjection())
            .ToListAsync();
    }


    public async Task<IReadOnlyList<DocumentDto>> GetDocumentsByProperty(string propertyId, DocumentScope? scope = DocumentScope.PropertyHistory)
    { 
        // current owner can see the PropertyHistory belonging to property and their own docs (d.PropertyOwnership == only the person can see)

        var docs = await context.Documents
            .Where(d => d.IsActive 
                && d.ClientId == tenantService.GetCurrentClientId()
                && d.PropertyId == propertyId  
                && d.Scope == scope )
        .Select(DocumentExtensions.ToDtoProjection())
        .ToListAsync();

        return docs;

    }

    public async Task<IReadOnlyList<DocumentDto>> GetDocumentsByOwnership(string ownershipId,DocumentScope? scope)
    {
        var docs = await context.Documents
            .Where(d => d.IsActive 
                && d.ClientId == tenantService.GetCurrentClientId() 
                && d.PropertyOwnershipId == ownershipId 
                && (scope ==null || d.Scope == scope) )
        .Select(DocumentExtensions.ToDtoProjection())
        .ToListAsync();

        return docs;
    }

    public async Task<bool> HasAccessByScope(string clientId, string userId, string? propertyId, DocumentScope scope)
    {
        var userClientAccess = await context.UserClientAccess
        .Where(uca => uca.ClientId == clientId && uca.UserId == userId && uca.IsActive)
       .FirstOrDefaultAsync();
        if (userClientAccess == null) return false;

        if (userClientAccess.Role == "board_member")
        switch (scope)
        {
            case DocumentScope.Community:
            case DocumentScope.Public:
                return userClientAccess != null;
            case DocumentScope.OwnerTenure:
            case DocumentScope.PropertyHistory:
                if (propertyId == null) return false;

                var ownership = await context.PropertyOwnerships
                        .Include(po => po.Member)
                        .Where(po => po.PropertyId == propertyId
                                && po.Property.ClientId == clientId
                                && po.IsCurrent == true
                                && po.EndDate == null
                                && po.Member != null && po.Member.UserId == userClientAccess.UserId
                                && po.OwnershipType == OwnershipType.Primary)
                        .Select(po => po.MemberId)
                        .ToListAsync();
                if (ownership != null) return true;
                return false;
        }
        return false;

    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Document document)
    {
        //we can avoid getting the error by using the update method
        // in case of identical savechanges which in SaveAllAsync method will return false.
        context.Entry(document).State = EntityState.Modified;
    }
}
