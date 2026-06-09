using System;
using API.DTOs;
using API.DTOs.Prop;

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
     
        return property.ToDto();
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<ActionResult<IReadOnlyList<Property>>> GetCurrentPropertiesByOwner(string ownerId)
    {
        return Ok(await propertyRepository.GetMemberCurrentPropertiesAsync(ownerId));
    }

    [HttpPut("{propertyId}")]
    public async Task<ActionResult> UpdateProperty(string propertyId, PropertyUpdateDto propertyUpdateDto)
    {
        var property = await propertyRepository.GetPropertyAsync(propertyId);

        var userId = User.GetUserId();

        if (property == null) return BadRequest("Could not locate property to update");

        if (propertyUpdateDto.Address == string.Empty) return  BadRequest(propertyUpdateDto) ;

        property.Address = propertyUpdateDto.Address ?? property.Address;
        property.Unit = propertyUpdateDto.Unit ?? property.Unit;
        property.City = propertyUpdateDto.City ?? property.City;
        property.State = propertyUpdateDto.State ?? property.State;
        property.ZipCode = propertyUpdateDto.ZipCode ?? property.ZipCode;
        property.Country = propertyUpdateDto.Country ?? property.Country;

        if (propertyUpdateDto.IsSameAddress)
        {
            property.MailAddress = propertyUpdateDto.Address ?? property.MailAddress;
            property.MailUnit = propertyUpdateDto.Unit ?? property.MailUnit;
            property.MailCity = propertyUpdateDto.City ?? property.MailCity;
            property.MailState = propertyUpdateDto.State ?? property.MailState;
            property.MailZipCode = propertyUpdateDto.ZipCode ?? property.MailZipCode;
            property.MailCountry = propertyUpdateDto.Country ?? property.MailCountry;

        }
        else
        {
            property.MailAddress = propertyUpdateDto.MailAddress ?? property.MailAddress;
            property.MailUnit = propertyUpdateDto.MailUnit ?? property.MailUnit;
            property.MailCity = propertyUpdateDto.MailCity ?? property.MailCity;
            property.MailState = propertyUpdateDto.MailState ?? property.MailState;
            property.MailZipCode = propertyUpdateDto.MailZipCode ?? property.MailZipCode;
            property.MailCountry = propertyUpdateDto.MailCountry ?? property.MailCountry;
        }

        property.LotNumber = propertyUpdateDto.LotNumber ?? property.LotNumber;
        property.AssignedParking = propertyUpdateDto.AssignedParking ?? property.AssignedParking;
        property.SquareFeet = propertyUpdateDto.SquareFeet ?? property.SquareFeet;
        property.Bedrooms = propertyUpdateDto.Bedrooms ?? property.Bedrooms;
        property.Bathrooms = propertyUpdateDto.Bathrooms ?? property.Bathrooms;
        property.IsRented = propertyUpdateDto.IsRented;

        property.IsSameAddress = propertyUpdateDto.IsSameAddress;   
 
        property.LastUpdatedById = userId;
        property.LastUpdated = DateTime.UtcNow;



        if (await propertyRepository.SaveAllAsync()) return Ok();

        return BadRequest("Error while saving property");
    }
}

