using System;
using API.Entities;
using Microsoft.OpenApi.Writers;

namespace API.Helpers;

public class DocumentParams
{
    public DocumentScope? Scope { get; set; } = null;
    public DocumentCategory? Category { get; set; } = null;

    public String? MemberId { get; set; }
    public String? PropertyId { get; set; } 
    public String? PropertyOwnershipId { get; set; }   
}
