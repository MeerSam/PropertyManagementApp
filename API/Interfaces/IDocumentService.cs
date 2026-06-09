using System;
using API.DTOs.Docs;

namespace API.Interfaces;

public interface IDocumentService
{
    Task<DocumentDto> UploadDocumentAsync(IFormFile file);

    Task<DocumentDto> GetMemberDocuments(string memberId);

    Task<DocumentDto> GetPropertyDocuments(string propertyId);

    Task<DocumentDto> GetOwnerDocuments(string ownershipId);

    Task<DocumentDto> RequestViewDownload(string documentId, string requestMode);
 
}