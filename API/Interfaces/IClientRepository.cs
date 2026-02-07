using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IClientRepository
{
    void Update(Client client); // method does not return anything

    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<Client>> GetClientsAsync();

    Task<Client?> GetClientByIdAsync(string id);

    Task<IReadOnlyList<UserClientAccessInfoDto>?> GetClientsByUserIdAsync(string userId);



}
