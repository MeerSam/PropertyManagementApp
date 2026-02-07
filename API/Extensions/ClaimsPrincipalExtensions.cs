using System;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    // making a static class as it will be an extension 
    public static string GetClientId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("ClientId") 
        ?? throw new  Exception("Cannot get Client from token");
    }

    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new Exception("Cannot get users id from token"); // ?? null coalscening operator 
    }


public static string GetMemberId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("MemberId")
        ?? throw new Exception("Cannot get users id from token"); // ?? null coalscening operator 
    }
}
