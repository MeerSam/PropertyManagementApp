using System;
using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    // Service for Issuing tokens
    string CreateToken(AppUser user);

    

}
