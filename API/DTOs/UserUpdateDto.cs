using System;

namespace API;

public class UserUpdateDto
{
    public required string UserId { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Description { get; set; }

    public DateTime DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public string? Gender { get; set; }

}
