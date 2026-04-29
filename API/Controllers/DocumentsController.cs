using API.Data;
using API.DTOs.Docs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class DocumentsController(AppDbContext context,
    IDocumentAccessService documentAccessService,
    IDocumentStorageService storage,
    IDocumentRepository documentRepository,
    IPropertyRepository propertyRepository,
    IUserClientRepository loginAccess) : BaseApiController
{
    [HttpPost("upload")]
    public async Task<ActionResult<DocumentDto>> Upload([FromForm] UploadDocumentDto uploadDocDto, IFormFile file)
    {
        //  file will not come through body but [FromForm] to tell our API controller where to go looking for
        var userId = User.GetUserId();
        var clientId = User.GetClientId();

        // Signin and check access
        var canUpload = await documentAccessService.CanUploadAsync(userId, clientId, uploadDocDto.Scope, uploadDocDto.PropertyId, uploadDocDto.PropertyOwnershipId);

        if (!canUpload) return Unauthorized();

        // 2 — validate file
        var allowedTypes = new[]
        {
            "application/pdf", "image/jpeg", "image/png",
            "application/msword",
            "application/vnd.openxmlformats-officedocument" +
            ".wordprocessingml.document"
        };

        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest("File type not allowed.");

        if (file.Length > 25 * 1024 * 1024)
            return BadRequest("File exceeds 25MB limit.");


        // 3 — build storage key
        var safeFileName = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";


        var storageKey = uploadDocDto.Scope switch
        {
            DocumentScope.Community => StorageKeyBuilder.Community(clientId,
                uploadDocDto.Category,
                safeFileName),
            DocumentScope.PropertyHistory => StorageKeyBuilder.PropertyHistory(clientId,
                uploadDocDto.PropertyId!,
                uploadDocDto.Category,
                safeFileName),
            DocumentScope.OwnerTenure => StorageKeyBuilder.OwnerTenure(clientId,
                uploadDocDto.PropertyId!,
                uploadDocDto.PropertyOwnershipId!,
                uploadDocDto.Category,
                safeFileName),

            _ => throw new InvalidOperationException("Error during the storage key builder")

        };

        // 4 — upload to Supabase        
        await storage.UploadAsync(file, safeFileName, file.ContentType, storageKey);

        // 5 — handle versioning (mark old doc as superseded)
        if (!string.IsNullOrEmpty(uploadDocDto.SupersededDocumentId))
        {
            var old = await context.Documents.FindAsync(uploadDocDto.SupersededDocumentId);
            if (old != null)
            {
                old.IsSuperseded = true;
                // SupersededById set after we have the new doc's Id below
            }
        }


        // 6 — save Document entity
        var doc = new Document
        {
            ClientId = clientId,
            Title = uploadDocDto.Title,
            Description = uploadDocDto.Description,
            Scope = uploadDocDto.Scope,
            Category = uploadDocDto.Category,
            PropertyId = uploadDocDto.PropertyId,
            PropertyOwnershipId =   uploadDocDto.PropertyOwnershipId ,
            StorageKey = storageKey,
            FileName = file.FileName,
            MimeType = file.ContentType,
            FileSizeBytes = file.Length,
            UploadedByUserId = userId,
            Notes = uploadDocDto.Notes
        };

        context.Documents.Add(doc);

        // Wire up versioning chain
        if (!string.IsNullOrEmpty(uploadDocDto.SupersededDocumentId))
        {
            var old = await context.Documents.FindAsync(uploadDocDto.SupersededDocumentId);
            if (old != null) old.SupersededById = doc.Id;
        }

        await context.SaveChangesAsync();
        return doc.ToDto();

    }

    [HttpGet("community")]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetCommunityDocuments()
    {
        var userId = User.GetUserId();
        var clientId = User.GetClientId();

        var hasAccess = await loginAccess.GetAccessByIdAsync(clientId, userId);
        // Any active UserClientAccess for this client can view
        if (hasAccess == null) return Forbid("Unable request community docs based on the credentials");

        return Ok(await documentRepository.GetCommunityDocumentsByClient(clientId));
    }

    [HttpGet("property/{propertyId}/history")]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetDocuments(string propertyId)
    {
        var userId = User.GetUserId();
        var clientId = User.GetClientId();

        var access = await loginAccess.GetAccessByIdAsync(clientId, userId);
        if (access == null) return Unauthorized("You do not have access to documents you requested.");


        // Managers, admins, board can always list
        // Owners must be current primary owner of this property
        bool canView = access.Role is "admin" or "property_manager" or "board_member";

        if (!canView && access.Role == "owner")
        {
            canView = await propertyRepository.IsPrimaryOwner(clientId, userId, propertyId);
        }

        if (!canView) return Forbid();

        // This only provides docs by the logged in Users ID if the User is a Owner they'll receive all documents 
        var documents = await documentRepository.GetDocumentsByProperty(propertyId, DocumentScope.PropertyHistory);

        return Ok(documents);

    }
    [HttpGet("ownership/{ownershipId}/docs")]
    public async Task<ActionResult<IReadOnlyList<DocumentDto>>> GetOwnerDocuments(string ownershipId)
    {
        var userId = User.GetUserId();
        var clientId = User.GetClientId();

        var access = await loginAccess.GetAccessByIdAsync(clientId, userId);
        if (access == null) return Unauthorized("You do not have access to documents you requested.");


        // Managers, admins, board can always list
        // Owners must be current primary owner of this property
        bool canView = access.Role is "admin" or "property_manager";

        if (!canView && (access.Role is "owner" or "board_member"))
        {
            var member = await context.Members
            .Where(m => m.UserId == userId && m.ClientId == clientId)
            .FirstOrDefaultAsync();

            if (member == null) return NotFound("Owner details Not found");

            var ownership = await context.PropertyOwnerships
                        .Include(po => po.Member)
                        .Where(po => po.Id ==  ownershipId 
                                && po.Property.ClientId == clientId
                                && po.Member != null && po.MemberId == member.Id
                                && po.OwnershipType == OwnershipType.Primary)
                        .Select(po => po.MemberId)
                        .FirstOrDefaultAsync();

            canView = ownership != null && member != null;
        }

        if (!canView) return Forbid();


        // This only provides docs by the logged in Users ID if the User is a Owner they'll receive all documents 
        var documents = await documentRepository.GetDocumentsByOwnership(ownershipId, DocumentScope.OwnerTenure);

        return Ok(documents);

    }


    [HttpGet("{id}/url")]
    public async Task<ActionResult<DocumentUrlDto>> GetUrl(string id, [FromQuery] string mode = "preview")
    {
        var userId = User.GetUserId();
        var clientId = User.GetClientId();

        var access = await loginAccess.GetAccessByIdAsync(clientId, userId);
        if (access == null) return Unauthorized("You have no access to the info.");
        var doc = await context.Documents
                    .FirstOrDefaultAsync(d => d.Id == id);

        if (doc is null) return NotFound();

        // Always run the full access check before generating any URL
        if (!await documentAccessService.CanViewAsync(userId, doc)) return Forbid("You do not have access to documents you requested.");

        var url = await storage.GeneratePresignedUrlAsync(doc.StorageKey, 15);

        // For download mode, append content-disposition hint
        // (Supabase supports ?download=filename param)
        if (mode == "download")
            url += $"&download={Uri.EscapeDataString(doc.FileName)}";

        return Ok(new DocumentUrlDto
        {
            Url = url,
            Mode = mode,
            ExpiresInMinutes = 15
        });

    }


}
