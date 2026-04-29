using System;
using API.Entities;


namespace API.DTOs.Docs;

public class UploadDocumentDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DocumentScope Scope { get; set; }
    public DocumentCategory Category { get; set; }
    // Required when Scope != Community
    public string? PropertyId { get; set; }
    // Required when Scope == OwnerTenure
    public string? PropertyOwnershipId { get; set; }
    public string? Notes { get; set; }
    // SupersededDocumentId — if re-uploading an updated version
    public string? SupersededDocumentId { get; set; }

}
