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
            DisplayName = member.User.DisplayName,
            Email = member.User.Email ,
            UserId = member.UserId,
            MemberId = member.Id 
        };
    }

}
