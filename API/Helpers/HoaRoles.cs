
using System;

namespace API.Helpers;

public static class HoaRoles
{
    public const string Admin = "admin";
    public const string BoardMember = "board_member";
    public const string PropertyManager = "property_manager";
    public const string Owner = "owner";
    public const string Resident = "resident"; // same as tenant/renter

    public static readonly string[] PropertyManagementRoles =
    {
        Admin, 
        PropertyManager
    };
    public static readonly string[] priviledgedRoles =
    {
        Admin,
        BoardMember,
        PropertyManager
    };

    public static readonly string[] sysAdminRoles =
    {
        Admin
    };
    public static readonly string[] internalAdminRoles =
    {
        Admin,
        BoardMember 
    }; 

}