using System;

namespace API.Entities;

public class Document
{ 
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public required string Title { get; set; }
    public string? Description { get; set; }

    public DocumentScope Scope { get; set; }
    public DocumentCategory Category { get; set; }

    // Unit-level targeting (null for Community docs)
    public string? PropertyId { get; set; }
    public Property? Property { get; set; }

    // Tenure-level targeting (only set for OwnerTenure scope)
    public string? PropertyOwnershipId { get; set; }
    public PropertyOwnership? PropertyOwnership { get; set; }

    // Storage — never store a public URL
    public required string StorageKey { get; set; }     // e.g. clients/abc/properties/xyz/deed.pdf
    public required string FileName { get; set; }
    public required string MimeType { get; set; }
    public long FileSizeBytes { get; set; }

    // Versioning
    public bool IsActive { get; set; } = true;
    public bool IsSuperseded { get; set; } = false;
    public string? SupersededById { get; set; }         // points to the newer Document.Id
    public Document? SupersededBy { get; set; }

    // Audit
    public required string UploadedByUserId { get; set; }
    public AppUser UploadedBy { get; set; } = null!;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; } 
}

public enum DocumentScope
{
    Public,
    Community,        // All active members of this Client
    PropertyHistory,  // Current Primary owner + board + managers
    OwnerTenure       // Only that specific tenure's Primary owner + managers
}

public enum DocumentCategory
{
    // Community
    HOAGuidelines,
    MeetingMinutes,
    Budget,
    Newsletter,
    Rules,
    MeetingNotice,
    Other,
    // Property
    DeedOrOwnership,
    ViolationNotice,
    MaintenanceRecord,
    InspectionReport,
    ArchitecturalChangeRequest,
    TenantDocument,
    RentalAgreement,
    Miscellaneous
}