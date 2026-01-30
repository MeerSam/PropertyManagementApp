using System;
using System.Security.Claims;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API.Extensions;

public static class ClaimsPrincipalExtensions
{
    // making a static class as it will be an extension 
    public static string GetClientId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("client") 
        ?? throw new  Exception("Cannot get Client from token");
    }

}
