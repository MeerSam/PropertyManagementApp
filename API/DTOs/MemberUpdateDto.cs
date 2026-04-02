using System;

namespace API.DTOs;

public class MemberUpdateDto
{
    public   string? Email { get; set; }
    public   string? DisplayName { get; set; }
    public   string? FirstName { get; set; }
    public   string? LastName { get; set; }
    public string? Description { get; set; }

}
