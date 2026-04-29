using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IPropertyRepository
{
    Task<IReadOnlyList<Property>> GetPropertiesAsync();
    Task<Property?> GetPropertyAsync(string propertyId);

    Task<IReadOnlyList<PropertyDto>> GetMemberCurrentPropertiesAsync(string memberId);

    Task<bool> IsPrimaryOwner(string clientId, string userId, string propertyId);
    void Update(Property property); // method does not return anything

    Task<bool> SaveAllAsync();

}
