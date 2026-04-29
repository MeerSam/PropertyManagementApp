using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class UserClientRespository(AppDbContext context) : IUserClientRepository
{
    public async Task<UserClientAccess?> GetAccessByIdAsync(string clientId, string userId)
    {
       return await context.UserClientAccess   
        .Where(uca => uca.ClientId == clientId &&  uca.UserId== userId && uca.IsActive  ) 
        .FirstOrDefaultAsync(); 
    }

    public async Task<UserClientAccess?> GetDocumentAccessById(string clientId, string userId, Document document)
    {
        return await context.UserClientAccess   
        .Where(uca => uca.ClientId == clientId 
            &&  uca.UserId== userId 
            && uca.IsActive
            && document.ClientId == clientId) 
        .FirstOrDefaultAsync(); 
    }

    public async Task<bool> SaveAllAsync()
    {
        return await  context.SaveChangesAsync() > 0;
    }

    public void Update(UserClientAccess ucAcess)
    {
        //we can avoid getting the error by using the update method
        // in case of identical savechanges which in SaveAllAsync method will return false.
        context.Entry(ucAcess).State = EntityState.Modified;
    }
}
