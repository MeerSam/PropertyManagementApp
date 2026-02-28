using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;

namespace API.Extensions;

public static class MemberExtensions
{
    public static Expression<Func<Member, MemberClientDto>> ToDtoProjection(ITokenService tokenService)
    {
         
        return member => new MemberClientDto
        {
            ClientId = member.ClientId,
            ClientName = member.Client.Name,
            DisplayName = member.User != null?  member.User.DisplayName : member.FirstName,
            Email =member.User != null? member.User.Email: member.Email,
            UserId = member.User != null?  member.UserId  :null,
            MemberId = member.Id 
        };
    }

}
