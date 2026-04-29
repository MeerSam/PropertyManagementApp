using System;

namespace API.DTOs.Docs;

public class DocumentUrlDto
{
    public string Url { get; set; } = null!;
    public string Mode { get; set; } = null!;   // "preview" | "download"
    public int ExpiresInMinutes { get; set; }
}
