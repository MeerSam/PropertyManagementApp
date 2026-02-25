using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class ClientRepository(AppDbContext context) : IClientRepository
{
    public async Task<Client?> GetClientByIdAsync(string id)
    {
        return await context.Clients.FindAsync(id);
    }

    public async Task<IReadOnlyList<UserClientAccessInfoDto>?> GetClientsByUserIdAsync(string userId)
    {
        var clients = await context.UserClientAccess
            .Include(uca => uca.Client)
            .Include(uca => uca.User)
            .ThenInclude(uca => uca.Members)
            .Where(uca => uca.UserId == userId && uca.Client.IsActive == true)
            .Select(uca => uca.ToDto())
            .ToListAsync();
        return clients;
    }

    public async Task<IReadOnlyList<Client>> GetClientsAsync()
    {
        return await context.Clients.ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0; // number of changes > 0
    }

    public void Update(Client client)
    {
        //we can avoid getting the error by using the update method
        // in case of identical savechanges which in SaveAllAsync method will return false.
        context.Entry(client).State = EntityState.Modified;
    }
}
