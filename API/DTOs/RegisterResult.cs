using API.DTOs.Auth;

namespace API.DTOs;

public class RegisterResult
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
    public UserDto? User { get; set; }
}
 