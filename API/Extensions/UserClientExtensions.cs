using System;
using System.Linq.Expressions;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;
/// <summary>
/// 
/// </summary>
public static class UserClientExtensions
{

    // because this class is an extention we made it static 
    //meaning we do not need to create an instance of it and can directly use it 
    // note: cannot use dependency injection therefore  the TokenService was passed as parameter

    public static UserClientAccessInfoDto ToDto(this UserClientAccess uca)
    { 
        return new  UserClientAccessInfoDto
        {
            UserId = uca.UserId,
            Email = uca.User.Email!,
            DisplayName = uca.User.DisplayName,
            ImageUrl = uca.User.ImageUrl,
            ClientId = uca.ClientId,
            ClientName = uca.Client.Name,
            MemberId = uca.User.Members.FirstOrDefault(x => x.ClientId == uca.ClientId)?.Id,
            HasMemberProfile = uca.User.Members.FirstOrDefault(x => x.ClientId == uca.ClientId)?.Id !=null
        };
    }
    public static UserForEditDto ToUserDto(this UserClientAccess uca)
    { 
        var member = uca.User.Members
        .FirstOrDefault(m => m.ClientId == uca.ClientId);

        return new  UserForEditDto
        {
            Id = uca.UserId,
            Email = uca.User.Email!,
            DisplayName = uca.User.DisplayName,
            ImageUrl = uca.User.ImageUrl,
            ClientId = uca.ClientId,
            ClientName = uca.Client.Name,
            MemberId = member?.Id,
            IsMemberLinked = member != null,
            FirstName = uca.User.FirstName,
            LastName = uca.User.LastName, 
            DateOfBirth = uca.User.DateOfBirth,
            Gender = uca.User.Gender,
            Role = uca.Role,
            IsActive = uca.User.IsActive && uca.IsActive,
            LastUpdated = uca.User.LastUpdated       
        };
    }

    public static Expression<Func<UserClientAccess, UserClientAccessInfoDto>> ToDtoProjection()
    {
         
        return uca => new  UserClientAccessInfoDto
        {
            UserId = uca.UserId,
            Email = uca.User.Email!,
            DisplayName = uca.User.DisplayName,
            ImageUrl = uca.User.ImageUrl,
            ClientId = uca.ClientId,
            ClientName = uca.Client.Name,
            MemberId = uca.Client.Members
                .Where(m => m.UserId== uca.UserId)
                .Select(m => m.Id)
                .FirstOrDefault(),                
            HasMemberProfile = uca.User.Members
                .Any(m =>  m.UserId== uca.UserId) 
        };
    }

    public static Expression<Func<UserClientAccess, UserForEditDto>> ToUserDtoProjection()
    {
         
        return uca => new  UserForEditDto
        {
            Id = uca.UserId,
            Email = uca.User.Email!,
            DisplayName = uca.User.DisplayName,
            ImageUrl = uca.User.ImageUrl,
            ClientId = uca.ClientId,
            ClientName = uca.Client.Name,
            MemberId = uca.Client.Members
                .Where(m => m.UserId== uca.UserId)
                .Select(m => m.Id)
                .FirstOrDefault(),                
            IsMemberLinked = uca.User.Members
                .Any(m =>  m.UserId== uca.UserId) ,  
            FirstName = uca.User.FirstName,
            LastName = uca.User.LastName, 
            DateOfBirth = uca.User.DateOfBirth,
            Gender = uca.User.Gender,
            Role = uca.Role,
            IsActive = uca.User.IsActive,
            IsClientAccessActive = uca.IsActive,
            LastUpdated = uca.User.LastUpdated  
        };
    }
}

