using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class PropertiesController(IPropertyRepository propertyRepository) : BaseApiController
{
    [HttpGet] //https://localhost:5001/api/properties/prop-id
    public async Task<ActionResult<IReadOnlyList<Property>>> GetProperties()
    {
        return Ok(await propertyRepository.GetPropertiesAsync());
    }
    [HttpGet("{id}")] //https://localhost:5001/api/properties/prop-id
    public async Task<ActionResult<PropertyDto>> GetProperty(string id)
    {
        var property = await propertyRepository.GetPropertyAsync(id);
        if (property == null) return NotFound(" The requested info was not found");
        var dtoProperty = new PropertyDto
        {
            Id = property.Id,
            Address = property.Address,
            Unit = property.Unit,
            Bedrooms = property.Bedrooms,
            Bathrooms =property.Bathrooms,
            IsRented = property.IsRented,   
            SquareFeet = property.SquareFeet,            
            Ownerships = [.. property.Ownerships
                .Select(o => new PropertyOwnershipDto
                {
                    Id = o.Id,
                    PropertyId = o.PropertyId,
                    MemberId = o.MemberId,
                    StartDate =o.StartDate,
                    EndDate =o.EndDate,
                    OwnershipType =o.OwnershipType.ToString(),
                    OwnershipPercentage =o.OwnershipPercentage,
                    IsCurrent= o.IsCurrent

                })],
            CurrentOwners = [.. property.Ownerships
                .Where(o => o.IsCurrent)
                .Select(o => new MemberDto
                {
                    Id = o.Member.Id,
                    DisplayName = o.Member.DisplayName,
                    Email = o.Member.Email,
                    FirstName = o.Member.FirstName,
                    LastName = o.Member.LastName ,
                    ClientId =o.Member.ClientId
                })]
        };

        return dtoProperty;
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<IReadOnlyList<Property>>> GetCurrentPropertiesByOwner(string ownerId)
    {
        return Ok(await propertyRepository.GetMemberCurrentPropertiesAsync(ownerId));
    }
}
