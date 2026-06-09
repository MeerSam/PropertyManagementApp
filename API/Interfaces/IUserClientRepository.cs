using System;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using Supabase.Gotrue;

namespace API.Interfaces;

public interface IUserClientRepository
{
    void Update(UserClientAccess ucAcess); // method does not return anything
    Task<bool> SaveAllAsync();
    Task<UserClientAccess?> GetAccessByIdAsync(string clientId, string userId);

    Task<UserClientAccess?> GetDocumentAccessById(string clientId, string userId, Document document);

    Task<UserForEditDto> GetUserForUpdate(string clientId, string userId);

    Task<IReadOnlyList<UserForEditDto>> GetUsers(string clientId);
}
