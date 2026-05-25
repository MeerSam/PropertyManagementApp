using System;
using API.Entities;
using API.Helpers;


namespace API.Interfaces;

public interface IAuthorizationService
{
    
    // Document Permissions
    Task<bool> CanViewDocumentAsync(string userId, Document document);
    Task<bool> CanUploadDocumentAsync(string userId, string clientId,
                              DocumentScope scope, 
                              string? propertyId, 
                              string? propertyOwnershipId);



    // Property Permissions
    Task<bool> CanViewPropertyAsync(UserAccess access, string propertyId); // owner, admin, pm and board
    Task<bool> CanViewLimitedPropertyAsync(UserAccess access, string propertyId); // tenant, resident who isnt a primary owner 
    Task<bool> CanCreatePropertyAsync(UserAccess access); // admin, pm and board
    Task<bool> CanUpdatePropertyAsync(UserAccess access, string propertyId); // admin, pm and board
    Task<bool> CanDeletePropertyAsync(UserAccess access, string propertyId); // only admin
}