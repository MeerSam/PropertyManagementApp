using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtensions
{
 public static UserDto ToDto(this AppUser user,  ITokenService tokenService)
    {
        return new UserDto
        {
            Email = user.Email,
            Id = user.Id,
            DisplayName = user.DisplayName,
            ClientId = "",
            Token =  tokenService.CreateToken(user)
        };
    }
}
