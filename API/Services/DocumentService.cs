using API.DTOs.Docs;
using API.Interfaces;

namespace API.Services;

public class DocumentService : IDocumentService
{
    public Task<DocumentDto> GetMemberDocuments(string memberId)
    {
        throw new NotImplementedException();
    }

    public Task<DocumentDto> GetOwnerDocuments(string ownershipId)
    {
        throw new NotImplementedException();
    }

    public Task<DocumentDto> GetPropertyDocuments(string propertyId)
    {
        throw new NotImplementedException();
    }

    public Task<DocumentDto> RequestViewDownload(string documentId, string requestMode)
    {
        throw new NotImplementedException();
    }

    public Task<DocumentDto> UploadDocumentAsync(IFormFile file)
    {
        throw new NotImplementedException();
    }
}