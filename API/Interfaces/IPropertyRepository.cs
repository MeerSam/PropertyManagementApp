using System;
using API.Entities;

namespace API.Interfaces;

public interface IPropertyRepository
{
    Task<IReadOnlyList<Property>> GetPropertiesAsync();
    Task<Property?> GetPropertyAsync(string propertyId);

    Task<IReadOnlyList<Property>> GetMemberCurrentPropertiesAsync(string memberId);
    void Update(Property property); // method does not return anything

    Task<bool> SaveAllAsync();

}
