using System;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class PropertyRepository(AppDbContext context, ITenantService tenantService) : IPropertyRepository
{
    public async Task<IReadOnlyList<Property>> GetMemberCurrentPropertiesAsync(string memberId)
    {
        var clientId = tenantService.GetCurrentClientId();

        return await context.PropertyOwnerships
            .Include(po => po.Member)
            .Include(po => po.Property)
            .Where(po => po.MemberId == memberId   
                    && po.Property.ClientId == clientId
                    && po.IsCurrent && po.EndDate ==null)
            .Select(po => po.Property)
            .Distinct()
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Property>> GetPropertiesAsync()
    {   
        var clientId = tenantService.GetCurrentClientId();
        var properties = await context.Properties
            .Where(p => p.ClientId == clientId)
            .ToListAsync();
        return properties;
    }

    public async Task<Property?> GetPropertyAsync(string propertyId)
    {
        var property = await context.Properties
            .Include(p => p.Ownerships) 
                .ThenInclude(po => po.Member)
            .Where(p => p.ClientId == tenantService.GetCurrentClientId())
            .Where(p => p.Id == propertyId) 
            .SingleOrDefaultAsync();

        if (property != null)
        {
            if (property.ClientId != tenantService.GetCurrentClientId())
                throw new UnauthorizedAccessException("You do not have access to this property");
        }
        return property;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Property property)
    {
        //we can avoid getting the error by using the update method
        // in case of identical savechanges which in SaveAllAsync method will return false.
        context.Entry(property).State = EntityState.Modified;
    }



}
