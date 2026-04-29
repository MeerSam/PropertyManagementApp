using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IUserClientRepository
{
    void Update(UserClientAccess ucAcess); // method does not return anything
    Task<bool> SaveAllAsync();
    Task<UserClientAccess?> GetAccessByIdAsync(string clientId, string userId);

    Task<UserClientAccess?> GetDocumentAccessById(string clientId, string userId, Document document);

}
