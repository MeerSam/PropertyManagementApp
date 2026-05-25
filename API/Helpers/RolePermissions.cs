using System;

namespace API.Helpers;

public static class RolePermissions
{
    public static bool CanViewAnyDocs(string role) => 
        role is HoaRoles.Admin 
        or HoaRoles.PropertyManager 
        or HoaRoles.BoardMember;

    public static bool CanViewCommunityDocs(string role) =>
        role is HoaRoles.Admin
        or HoaRoles.PropertyManager
        or HoaRoles.BoardMember
        or HoaRoles.Owner
        or HoaRoles.Resident; 

    public static bool CanUploadDocuments(string role) =>
        HoaRoles.PropertyManagementRoles.Contains(role);

    public static bool CanApproveRequests(string role) =>
        role is HoaRoles.Admin or HoaRoles.BoardMember;

}