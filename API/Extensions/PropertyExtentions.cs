using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;

namespace API.Extensions;

public static class PropertyExtensions
{
    public static PropertyDto ToDto(this Property property)
    {
        return new PropertyDto
        {
            Id = property.Id,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            IsRented = property.IsRented,
            SquareFeet = property.SquareFeet,
            Address = property.Address,
            LotNumber = property.LotNumber,
            AssignedParking = property.AssignedParking,
            Unit = property.Unit,
            City = property.City,
            State = property.State,
            Country = property.Country,
            ZipCode = property.ZipCode,
            MailAddress = property.MailAddress,
            MailUnit = property.MailUnit,
            MailCity = property.MailCity,
            MailState = property.MailState,
            MailCountry = property.MailCountry,
            MailZipCode = property.MailZipCode,
            LastUpdated = property.LastUpdated,
            LastUpdatedBy = property.LastUpdatedBy?.DisplayName ?? string.Empty,
            ClientId = property.ClientId,
            IsSameAddress = property.IsSameAddress,
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
                    IsCurrent= o.IsCurrent,
                    Member =  o.Member.ToInfoDto()
                })],
            CurrentOwners = [.. property.Ownerships
                .Where(o => o.IsCurrent)
                .Select(o => o.Member.ToInfoDto())]
        };
    }

    public static Expression<Func<Property, PropertyDto>> ToDtoProjection()
    {
        return property => new PropertyDto
        {
            Id = property.Id,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            IsRented = property.IsRented,
            SquareFeet = property.SquareFeet,
            Address = property.Address,
            LotNumber = property.LotNumber,
            AssignedParking = property.AssignedParking,
            Unit = property.Unit,
            City = property.City,
            State = property.State,
            Country = property.Country,
            ZipCode = property.ZipCode,
            MailAddress = property.MailAddress,
            MailUnit = property.MailUnit,
            MailCity = property.MailCity,
            MailState = property.MailState,
            MailCountry = property.MailCountry,
            MailZipCode = property.MailZipCode,
            LastUpdated = property.LastUpdated,
            LastUpdatedBy = property.LastUpdatedBy != null ?
                        property.LastUpdatedBy.DisplayName
                        : string.Empty,
            ClientId = property.ClientId,
            IsSameAddress = property.IsSameAddress,
            Ownerships = property.Ownerships.Select(o => new PropertyOwnershipDto
            {
                Id = o.Id,
                PropertyId = o.PropertyId,
                MemberId = o.MemberId,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                OwnershipType = o.OwnershipType.ToString(),
                OwnershipPercentage = o.OwnershipPercentage,
                IsCurrent = o.IsCurrent,
                Member = new MemberDto
                {
                    Id = o.Member.Id,
                    DisplayName = o.Member.DisplayName,
                    Email = o.Member.Email,
                    FirstName = o.Member.FirstName,
                    LastName = o.Member.LastName,
                    ClientId = o.Member.ClientId,
                    ClientName = o.Member.Client.Name,
                    ImageUrl = o.Member.ImageUrl,
                    UserId = o.Member.UserId
                }
            }).ToList(),

            CurrentOwners = property.Ownerships
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
                    UserId = o.Member.UserId,
                    ClientName = o.Member.Client.Name,
                }).ToList()
        };
    }
}