using System;
using System.Collections.ObjectModel;
using API.DTOs.Docs;
using API.Entities;

namespace API.Interfaces;

public interface IDocumentRepository
{
    void Update(Document document); // method does not return anything
    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<DocumentDto>> GetDocumentsByProperty(string propertyId, DocumentScope? scope);
    Task<IReadOnlyList<DocumentDto>> GetDocumentsByOwnership(string ownershipId, DocumentScope? scope);

    Task<IReadOnlyList<DocumentDto>> GetCommunityDocumentsByClient(string clientId );

    Task<Boolean> HasAccessByScope(string clientId, string userId, string? propertyId, DocumentScope scope);
 

}
