using API.Data;
using API.DTOs.Auth;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// AppDbContext context,
public class AccountController(UserManager<AppUser> userManager,
 ITokenService tokenService,
 IAuthService authService,
 AppDbContext context,
 IUserClientRepository userClientRepo,
 IEmailSender emailSender) : BaseApiController
{
    [Authorize]
    [HttpGet("users/{id}")]
    public async Task<ActionResult<UserForEditDto>> GetUser(string id)
    {
        //  file will not come through body but [FromForm] to tell our API controller where to go looking for
        var requestedUserId = User.GetUserId();
        var requestedUser = await context.UserClientAccess
        .Include(uca => uca.User)
        .Include(uca => uca.Client)

        .Where(uca => uca.ClientId == User.GetClientId() && uca.UserId == requestedUserId)
        .FirstOrDefaultAsync();
        var clientId = User.GetClientId();

        // Can View/Update
        if (requestedUser is null) return Unauthorized("Not authorized to view");
        if (requestedUserId != id && !HoaRoles.priviledgedRoles.Contains(requestedUser.Role)) return Unauthorized("Not authorized to view");

        // this method is for updating the users who are not member profiles
        var user = await userManager.FindByIdAsync(id);


        if (user == null) return BadRequest("Invalid User");

        return await userClientRepo.GetUserForUpdate(clientId, id);
    }
    [Authorize]
    [HttpGet("users")]
    public async Task<ActionResult<IReadOnlyList<UserForEditDto>>> GetUsers()
    {
        //  file will not come through body but [FromForm] to tell our API controller where to go looking for
        var userId = User.GetUserId();
        var clientId = User.GetClientId();

        // Can View/Update
        var user = await userManager.FindByIdAsync(userId);

        if (user is null) return Unauthorized("Not authorized to view");

        // this method is for updating the users who are not member profiles

        var result = await userClientRepo.GetUsers(clientId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("register")] // /api/acount/register    
    [ProducesResponseType(typeof(RegisterResponseDto), 200)]
    [ProducesResponseType(typeof(Exception), 400)]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterDto registerDto)
    {
        /// This Method is only allowed after a user logged in and has rights to register new users
        /// The Signup and Register is not available for new users 
        ///  must be created by Admin/Property Managers/Hoa Board after verifying the residency
        try
        {
            if (string.IsNullOrEmpty(registerDto.Email) ||
                string.IsNullOrEmpty(registerDto.Password) ||
                string.IsNullOrEmpty(registerDto.DisplayName)) return BadRequest("Email is required");

            // if (await EmailExists(registerDto.Email)) return BadRequest("Email Taken"); //removed: aspnet-identity
            // using var hmac = new HMACSHA512();//removed: aspnet-identity  

            var requestUserId = User.GetUserId();
            var clientId = User.GetClientId();
            var registerResult = await authService.RegisterAsync(registerDto, clientId);
            if (!registerResult.Succeeded)
            {
                foreach (var error in registerResult.Errors)
                {
                    ModelState.AddModelError("identity", error);
                }
                return ValidationProblem();
            }
            if (registerResult.User is null)
            {
                return BadRequest($"Something went wrong during registration. The User is not registered");
            }
            return new RegisterResponseDto
            {
                Success = !string.IsNullOrEmpty(registerResult.User?.Id),
                Id = registerResult.User?.Id,
                DisplayName = registerResult.User?.DisplayName,
                ClientName = registerResult.User?.ActiveClient?.ClientName ?? string.Empty,
                ActiveClient = registerResult.User?.ActiveClient,
                AppRole = "AppUser",
                Role = registerResult.User?.Role

            };

        }
        catch (System.Exception ex)
        {
            return BadRequest($"Something went wrong during registration {ex.Message}");
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
            // Step 1: Find user by email
            var user = await userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return Unauthorized("Invalid creadentials entered (email)");

            var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

            if (!result)
            {
                return Unauthorized("Invalid Username or Password");
            }


            var response = await authService.AuthenticateAsync(loginDto);

            if (response.AvailableClients == null) return Unauthorized(new AuthErrorResponseDto
            {
                Message = "You are not authorized to access any clients at this point."
            });
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


    // ---------------------------------------------------------
    // UPDATE CURRENT PASSWORD (PUT)
    // ---------------------------------------------------------
    [Authorize]
    [HttpPut("updatecreds")]
    public async Task<IActionResult> UpdateUserCredentials(CredentialsChangeDto creds)
    {
        var user = await userManager.FindByEmailAsync(creds.Email);

        var reqUser = await userManager.FindByIdAsync(User.GetUserId());

        if (user == null || reqUser == null)
            return NotFound("Invalid user and not Authorized to make change");

        // Get roles of the requester
        var reqRoles = await userManager.GetRolesAsync(reqUser);
        bool isSuperAdmin = reqRoles?.Contains("Super Admin") == true;

        // CASE 1: User changing their own password
        if (user.Id == creds.UserId && user.Id == reqUser.Id)
        {
            var valid = await userManager.CheckPasswordAsync(user, creds.CurrentPassword);
            if (!valid)
                return Unauthorized("Invalid password change request and not Authorized to make change.");
            var result = await userManager.ChangePasswordAsync(user, creds.CurrentPassword, creds.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password updated.");
        }
        // CASE 2: Super Admin changing someone else's password
        if (isSuperAdmin)
        {
            // Remove old password
            var remove = await userManager.RemovePasswordAsync(user);
            if (!remove.Succeeded)
                return BadRequest(remove.Errors);

            // Add new password
            var add = await userManager.AddPasswordAsync(user, creds.NewPassword);
            if (!add.Succeeded)
                return BadRequest(add.Errors);

            return Ok("Password updated by Admin.");
        }
 
        // CASE 3: Not allowed 
        return Unauthorized("You do not have permission to change this user's password.");
 
    }
    // ---------------------------------------------------------
    // FORGOT PASSWORD (POST)
    // ---------------------------------------------------------

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await userManager.FindByEmailAsync(forgotPasswordDto.Email);

        // Always return OK to avoid email enumeration
        if (user == null) return Ok(new { message = "If the email exists, a reset link has been sent." });

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        // Encode token for URL
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);

        var resetUrl = $"{forgotPasswordDto.ResetUrlBase}?email={user.Email}&token={encodedToken}";

        if (user.Email == null) return Ok(new { message = "If the email exists, a reset link has been sent." });


        await emailSender.SendEmailAsync(
            user.Email,
            "Reset Your Password",
            $"Click the link to reset your password: {resetUrl}");

        return Ok(new { message = "If the email exists, a reset link has been sent." });
    }

    // ---------------------------------------------------------
    //  RESET PASSWORD (POST)
    // ---------------------------------------------------------
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
        if (user == null)
            return BadRequest(new { message = "Invalid request." });

        var decodedToken = System.Web.HttpUtility.UrlDecode(resetPasswordDto.Token);

        var result = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Password reset failed.",
                errors = result.Errors.Select(e => e.Description)
            });
        }

        return Ok(new { message = "Password has been reset successfully." });
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken == null) return NoContent();

        var uca = await context.UserClientAccess
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
            && x.RefreshTokenExpiry > DateTime.UtcNow);

        if (uca == null) return Unauthorized();

        await SetRefreshTokenCookie(uca);

        var user = uca.User;
        return user.ToDto();
        // return user.ToDto(tokenService); 
    }
    [Authorize]
    [HttpPut("update")]
    public async Task<ActionResult> UpdateUser(UserUpdateDto userUpdateDto)
    {
        var currentUserId = User.GetUserId();
        var clientId = User.GetClientId();

        var userToUpdate = await userManager.FindByIdAsync(userUpdateDto.UserId);
        if (userToUpdate == null) return NotFound("Invalid user");

        // Verify user has access to the selected client
        var userClientAccess = await context.UserClientAccess
            .Where(uca => uca.ClientId == clientId
                && uca.UserId == currentUserId
                && uca.IsActive)
            .FirstOrDefaultAsync() ?? throw new UnauthorizedAccessException("You don't have access to this HOA community"); ;


        if (HoaRoles.priviledgedRoles.Contains(userClientAccess.Role))
        {
            // authorized
            userToUpdate.DisplayName = userUpdateDto.DisplayName ?? userToUpdate.DisplayName;
            userToUpdate.FirstName = userUpdateDto.FirstName ?? userToUpdate.FirstName;
            userToUpdate.LastName = userUpdateDto.LastName ?? userToUpdate.LastName;
            userToUpdate.Email = userUpdateDto.Email ?? userToUpdate.Email;
            userToUpdate.ImageUrl = userUpdateDto.ImageUrl ?? userToUpdate.ImageUrl;
            if (userUpdateDto.DateOfBirth != default)
            {
                userToUpdate.DateOfBirth = DateOnly.FromDateTime(userUpdateDto.DateOfBirth);
            }

        }
        var user = await context.Users
            .Include(u => u.Members)
            .Include(u => u.ClientAccess)
            .FirstAsync(u => u.Id == userToUpdate.Id);

        foreach (var member in user.Members)
        {
            member.DisplayName = userUpdateDto.DisplayName ?? member.DisplayName;
            member.FirstName = userUpdateDto.FirstName ?? member.FirstName;
            member.LastName = userUpdateDto.LastName ?? member.LastName;
            member.Email = userUpdateDto.Email ?? member.Email;
            member.Description = userUpdateDto.Description ?? member.Description;
            member.ImageUrl = userUpdateDto.ImageUrl ?? member.ImageUrl;
            if (userUpdateDto.DateOfBirth != default)
            {
                member.DateOfBirth = DateOnly.FromDateTime(userUpdateDto.DateOfBirth);
            }
        }
        if (await context.SaveChangesAsync() > 0) return NoContent();
        return BadRequest("Update could not be completed");
    }

    private async Task SetRefreshTokenCookie(UserClientAccess uca)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        uca.RefreshToken = refreshToken;
        uca.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7); //make it a long live token
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



}
