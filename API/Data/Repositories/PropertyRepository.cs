using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class PropertyRepository(AppDbContext context, ITenantService tenantService) : IPropertyRepository
{
    public async Task<IReadOnlyList<PropertyDto>> GetMemberCurrentPropertiesAsync(string memberId)
    {
        var clientId = tenantService.GetCurrentClientId();

        return await context.PropertyOwnerships
            .Include(po => po.Member)
            .Include(po => po.Property)
                .ThenInclude(p => p.Ownerships)
                   .ThenInclude(o => o.Member)
            .Where(po => po.MemberId == memberId
                    && po.Property.ClientId == clientId
                    && po.IsCurrent && po.EndDate == null)
            .Select(po => new PropertyDto
            {
                Id = po.Property.Id,
                Address = po.Property.Address,
                Unit = po.Property.Unit,
                Bedrooms = po.Property.Bedrooms,
                Bathrooms = po.Property.Bathrooms,
                IsRented = po.Property.IsRented,
                SquareFeet = po.Property.SquareFeet,
                City= po.Property.City,
                State = po.Property.State, 
                Ownerships = po.Property.Ownerships
                    .Select(o => new PropertyOwnershipDto
                    {
                        Id = o.Id,
                        PropertyId = o.PropertyId,
                        MemberId = o.MemberId,
                        StartDate = o.StartDate,
                        EndDate = o.EndDate,
                        OwnershipType = o.OwnershipType.ToString(),
                        OwnershipPercentage = o.OwnershipPercentage,
                        IsCurrent = o.IsCurrent
                    })
                    .ToList(),
                CurrentOwners = po.Property.Ownerships
                    .Where(o => o.IsCurrent)
                    .Select(o => new MemberDto
                    {
                        Id = o.Member.Id,
                        DisplayName = o.Member.DisplayName,
                        Email = o.Member.Email,
                        FirstName = o.Member.FirstName,
                        LastName = o.Member.LastName,
                        ClientId = o.Member.ClientId,
                        ImageUrl = o.Member.ImageUrl,
                        UserId = o.Member.UserId
                    })
                    .ToList()
            })
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

    public async Task<bool> IsPrimaryOwner(string clientId, string userId, string propertyId)
    {

        var ownership = await context.PropertyOwnerships
                        .Include(po => po.Member)
                        .Where(po => po.PropertyId == propertyId
                                && po.Property.ClientId == clientId
                                && po.IsCurrent == true
                                && po.EndDate == null
                                && po.Member != null && po.Member.UserId == userId
                                && po.OwnershipType == OwnershipType.Primary)
                        .Select(po => po.MemberId)
                        .FirstOrDefaultAsync();

        if (ownership != null) return true;
        return false;
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
