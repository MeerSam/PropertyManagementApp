using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtensions
{
    // making a static class as it will be an extension 
/* When we make something static in C#, it means we do not need to create a new instance of it in

order to use its functionality. So I will not need to say where I'm going to use this in the account controller var app user extensions

equals new user equals new app user extensions. */
 public static async Task<UserDto> ToDto(this AppUser user,  ITokenService tokenService)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName ,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ImageUrl = user.ImageUrl, 
        };
    }
}
