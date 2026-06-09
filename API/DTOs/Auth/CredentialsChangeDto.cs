using System;

namespace API.DTOs.Auth;

 public class CredentialsChangeDto
{
    public string UserId { get; set; } =  "";

    public string Email { get; set; } =  "";

    public string CurrentPassword { get; set; } =  "";
    public string NewPassword { get; set; } =  "";

}


public class ResetPasswordDto
{
    public required string Email { get; set; }
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
}

 public class ForgotPasswordDto
 {
    public string Email { get; set; } =  ""; 

    // Angular will pass something like:
    // https://app.myhoa.com/reset-password
    public required string ResetUrlBase { get; set; }
    
 } 