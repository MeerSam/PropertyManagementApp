
namespace API.Entities;

public class AppUser
{

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsBoardMember { get; set; } = false;
    public bool IsAdminMember { get; set;} = false;
}
