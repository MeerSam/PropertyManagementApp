using System;
using API.Entities;

namespace API.DTOs.Docs;

public class DocumentDto
{
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DocumentScope Scope { get; set; }
    public DocumentCategory Category { get; set; }
    public string FileName { get; set; } = null!;
    public string MimeType { get; set; } = null!;
    public long FileSizeBytes { get; set; }
    public bool IsSuperseded { get; set; }
    public string? SupersededById { get; set; }
    public string UploadedByDisplayName { get; set; } = null!;
    public DateTime UploadedAt { get; set; }
    public string? Notes { get; set; }

}
