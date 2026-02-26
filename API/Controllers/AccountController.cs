using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(AppDbContext context, ITokenService tokenService, IAuthService authService) : BaseApiController
{
     
    [HttpPost("register")] // /api/acount/register    
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(typeof(Exception), 400)]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        /// This Method is only allowed after a user logged in and has rights to register new users
        /// The Signup and Register is not available for new users 
        ///  must be created by Admin/Property Managers/Hoa Board after verifying the residency
        try
        {
            if (string.IsNullOrEmpty(registerDto.Email) || string.IsNullOrEmpty(registerDto.Password) || string.IsNullOrEmpty(registerDto.DisplayName)) return BadRequest("Email is required");

            if (await EmailExists(registerDto.Email)) return BadRequest("Email Taken"); //removed: aspnet-identity
            using var hmac = new HMACSHA512();//removed: aspnet-identity  

            var requestUserId = User.GetUserId();
            var clientId = User.GetClientId();
            var result = await authService.RegisterAsync(registerDto, clientId); 

            return result;
        }
        catch (System.Exception ex)
        {
            return BadRequest($"Something went wrong during registration { ex.Message}");
        }
    }
    [HttpPost("login")]
    [ProducesResponseType(typeof(ClientSelectLoginResponseDto), 200)]
    [ProducesResponseType(typeof(AuthSuccessResponseDto), 200)]
    [ProducesResponseType(typeof(AuthErrorResponseDto), 401)]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var response = await authService.AuthenticateAsync(loginDto);

            if (response.AvailableClients == null) return BadRequest("Issues Accessing Clients");
            {
                if (response.AvailableClients.Count == 1)
                {
                    var selectRequest = new SelectClientRequestDto
                    {
                        ClientId = response.AvailableClients.First().ClientId,
                    };

                    var authResponse = await authService.CompleteLoginAsync(
                        response.SelectionToken,
                        selectRequest);

                    return Ok(authResponse);
                }
            }
            // User has multiple clients - return selection response
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new AuthErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthErrorResponseDto
            {
                Message = "An error occurred during login",
                Errors = [ex.Message]  //new List<string> { ex.Message }
            });
        }

    }
    /// <summary>
    /// Step 2: User selects a client - Returns JWT with ClientId and MemberId claims
    /// </summary>
    [HttpPost("selectclient")]
    [ProducesResponseType(typeof(AuthSuccessResponseDto), 200)]
    [ProducesResponseType(typeof(AuthErrorResponseDto), 401)]
    public async Task<ActionResult> SelectClient(SelectClientRequestDto request)
    {
        try
        {
            // Get selection token from Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new AuthErrorResponseDto
                {
                    Message = "Selection token is required"
                });
            }

            var selectionToken = authHeader.Substring("Bearer ".Length).Trim();

            // Complete login with the selection token
            var response = await authService.CompleteLoginAsync(selectionToken, request);

            return Ok(response);

        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new AuthErrorResponseDto
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new AuthErrorResponseDto
            {
                Message = "An error occurred while selecting client",
                Errors = [ex.Message]
            });
        }

    }

    [HttpPut("updateuser")]
    public async Task<IActionResult> UpdateUserData(LoginDto loginDto)
    {
        var user = await context.Users.SingleOrDefaultAsync(x => x.Email!.ToLower() == loginDto.Email.ToLower());
        if (user == null) return NotFound("Invalid user");
        using var hmac = new HMACSHA512();

        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));
        user.PasswordSalt = hmac.Key;

        await context.SaveChangesAsync();

        return Ok("UserUpdated");

    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) return NoContent();

        var user = await context.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
            && x.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null) return Unauthorized();

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);

    }


    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); //make it a long live token
        await context.SaveChangesAsync();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, //this cookie is not accessible from any kind of JavaScript, including our own client application.
            // We will not be able to get access to this cookie client side.
            Secure = true, // only sent over https and not over http
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

    }



    private async Task<bool> EmailExists(string email)
    {
        return await context.Users.AnyAsync(x => x.Email!.ToLower() == email.ToLower());
    }




}
