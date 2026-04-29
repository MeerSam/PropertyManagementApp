using System;
using System.Linq.Expressions;
using API.DTOs.Docs;
using API.Entities;

namespace API.Extensions;

public static class DocumentExtensions
{

    public static DocumentDto ToDto(this Document document)
    {
        var dto = new DocumentDto 
        {
            Id= document.Id,
            Title = document.Title,
            Description = document.Description,
            Scope = document.Scope,
            Category = document.Category,
            FileName = document.FileName,
            MimeType = document.MimeType,
            FileSizeBytes =document.FileSizeBytes,
            IsSuperseded= document.IsSuperseded,
            SupersededById=document.SupersededById,
            UploadedByDisplayName=document.UploadedBy?.DisplayName ?? string.Empty,
            UploadedAt=document.UploadedAt,
            Notes=document.Notes
        };
        return dto;
    }
    public static Expression<Func<Document, DocumentDto>> ToDtoProjection()
    {
        return document => new DocumentDto
        {
            Id= document.Id,
            Title = document.Title,
            Description = document.Description,
            Scope = document.Scope,
            Category = document.Category,
            FileName = document.FileName,
            MimeType = document.MimeType,
            FileSizeBytes =document.FileSizeBytes,
            IsSuperseded= document.IsSuperseded,
            SupersededById=document.SupersededById,
            UploadedAt=document.UploadedAt,
            Notes=document.Notes
        };
    }

}
