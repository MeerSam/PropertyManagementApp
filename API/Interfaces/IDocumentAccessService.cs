using System;
using API.Entities;

namespace API.Interfaces;

public interface IDocumentAccessService
{
    // Access logic — single place, all rules live here
    Task<bool> CanViewAsync(string userId, Document document);
    Task<bool> CanUploadAsync(string userId, string clientId,
                              DocumentScope scope,string? propertyId, string? propertyOwnershipId);

}
