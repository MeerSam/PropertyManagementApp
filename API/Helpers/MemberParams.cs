using System;

namespace API.Helpers;

public class MemberParams
{
    public string? CurrentClientId {get; set;}
     public string OrderBy { get; set; } = "lastActive";

}
