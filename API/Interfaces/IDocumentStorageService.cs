using System;
using API.Helpers;

namespace API.Interfaces;


public interface IDocumentStorageService
{
    Task<FileUploadResult> UploadAsync(IFormFile file, string fileName,
                             string mimeType, string storageKey);
    Task<string> GeneratePresignedUrlAsync(string storageKey,
                                               int expiryMinutes = 15);
    Task DeleteAsync(string storageKey);  // soft-delete only in practice
}
