using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // /api/acount/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.DisplayName)) return BadRequest("Email is required");

        if (await EmailExists(registerDto.Email)) return BadRequest("Email Taken"); //removed: aspnet-identity
        using var hmac = new HMACSHA512();//removed: aspnet-identity  

        var user = new AppUser
        {            
            Email = registerDto.Email,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            DisplayName = registerDto.DisplayName,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key,
            Gender = registerDto.Gender,
            DateOfBirth = registerDto.DateOfBirth

        };
        context.Users.Add(user); // ef track changes 
        await context.SaveChangesAsync();
        return user.ToDto(tokenService);

    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email!.ToLower() == loginDto.Email.ToLower());
        if (user == null) return Unauthorized("Invalid creadentials entered");
        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        try
        {
            for (var i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Username or Password");
            }
        }
        catch (System.Exception)
        {
            throw;
        }
        return user.ToDto(tokenService);
    }

    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email!.ToLower() == email.ToLower());
    }
}
