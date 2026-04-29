using System;
using API.Entities;

namespace API.Helpers;

public static class StorageKeyBuilder
{
    //Community
    public static string Community(string clientId, DocumentCategory category, string fileName)
    {
        return $"clients/{clientId}/community/{GetCategoryName(category)}/{fileName}";
    }
    // Property history doc (violations, maintenance, inspections)
    // clients/abc/properties/xyz/history/violations/notice-001.pdf
    public static string PropertyHistory(string clientId, string propertyId, DocumentCategory category, string fileName)
    {
        return $"clients/{clientId}/properties/{propertyId}/history/{GetCategoryName(category)}/{fileName}";
    }

    // Property history doc (violations, maintenance, inspections)
    // clients/abc/properties/xyz/history/violations/notice-001.pdf
    public static string OwnerTenure(string clientId, 
        string propertyId, 
        string ownershipId,
        DocumentCategory category, string fileName)
    {
        return $"clients/{clientId}/properties/{propertyId}/tenure/{ownershipId}/{GetCategoryName(category)}/{fileName}";
    }
    private static string GetCategoryName(DocumentCategory category) => category switch
    {
        DocumentCategory.ArchitecturalChangeRequest => "arc",
        DocumentCategory.Budget => "budget",
        DocumentCategory.DeedOrOwnership => "deeds",
        DocumentCategory.HOAGuidelines => "guidelines",
        DocumentCategory.InspectionReport => "inspections",
        DocumentCategory.MaintenanceRecord => "maintenance",
        DocumentCategory.MeetingMinutes => "minutes",
        DocumentCategory.MeetingNotice => "meeting-notice",
        DocumentCategory.Newsletter => "newsletter",
        DocumentCategory.Rules => "",
        DocumentCategory.ViolationNotice => "violation",
        _ => "other"
    };

}
