using System;
using System.Linq.Expressions;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using API.Services;

namespace API.Extensions;

public static class MemberExtensions
{

    public static MemberDto ToInfoDto(this Member member)
    {

        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            DisplayName = member.User != null ? member.User.DisplayName : member.FirstName,
            Email = member.User != null ? member.User.Email! : member.Email,
            DateOfBirth = member.DateOfBirth,
            Gender = member.User != null ? member.User.Gender! : member.Gender,
            ClientId = member.ClientId,
            PropertyOwnerships = [.. member.PropertyOwnerships
                .Select(o=> new PropertyOwnershipDto
                {
                Id = o.Id,
                PropertyId = o.PropertyId,
                MemberId = o.MemberId,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                OwnershipType = o.OwnershipType.ToString(),
                OwnershipPercentage = o.OwnershipPercentage,
                IsCurrent = o.IsCurrent,
                Property = new PropertyDto {
                    Id = o.PropertyId,
                    Address = o.Property.Address,
                    Unit = o.Property.Unit,
                    City = o.Property.City,
                    State = o.Property.State,  
                },
                })],
            UserId = member.User != null ? member.User.Id : member.UserId,
        };

        // MemberDto
        // PropertyOwnerships → PropertyOwnershipDto 
        // No PropertyDto (good) This is a tree, not a cycle.
    }
    public static MemberDto ToDto(this Member member)
    {

        return new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            DisplayName = member.User != null ? member.User.DisplayName : member.FirstName,
            Email = member.User != null ? member.User.Email! : member.Email,
            DateOfBirth = member.DateOfBirth,
            Gender = member.User != null ? member.User.Gender! : member.Gender,
            ClientId = member.ClientId,
            ClientName = member.Client.Name,
            PropertyOwnerships = [.. member.PropertyOwnerships
                .Select(o=> new PropertyOwnershipDto
                {
                Id = o.Id,
                PropertyId = o.PropertyId,
                MemberId = o.MemberId,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                OwnershipType = o.OwnershipType.ToString(),
                OwnershipPercentage = o.OwnershipPercentage,
                Property = new PropertyDto {
                    Id = o.PropertyId,
                    Address = o.Property.Address,
                    Unit = o.Property.Unit,
                    City = o.Property.City,
                    State = o.Property.State,  
                },
                IsCurrent = o.IsCurrent
                })],
            UserId = member.User != null ? member.User.Id : member.UserId,
        };
    }
    public static Expression<Func<Member, MemberDto>> ToMemberDtoProjection()
    { 
        return member => new MemberDto
        {
            Id = member.Id,
            FirstName = member.FirstName,
            LastName = member.LastName,
            DisplayName = member.User != null ? member.User.DisplayName : member.FirstName,
            Email = member.User != null ? member.User.Email! : member.Email,
            DateOfBirth = member.DateOfBirth,
            Gender = member.User != null ? member.User.Gender! : member.Gender,
            ClientId = member.ClientId,
            ClientName = member.Client.Name,
            PropertyOwnerships =  member.PropertyOwnerships
                .Select(o=> new PropertyOwnershipDto
                {
                     Id = o.Id,
                PropertyId = o.PropertyId,
                MemberId = o.MemberId,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                OwnershipType = o.OwnershipType.ToString(),
                OwnershipPercentage = o.OwnershipPercentage,
                IsCurrent = o.IsCurrent,
                Property = new PropertyDto {
                    Id = o.PropertyId,
                    Address = o.Property.Address,
                    Unit = o.Property.Unit,
                    City = o.Property.City,
                    State = o.Property.State,  
                },
                }).ToList(),
            UserId = member.User != null ? member.User.Id : member.UserId,
        };
    }
    public static Expression<Func<Member, MemberClientDto>> ToDtoProjection(ITokenService tokenService)
    {

        return member => new MemberClientDto
        {
            ClientId = member.ClientId,
            ClientName = member.Client.Name,
            DisplayName = member.User != null ? member.User.DisplayName : member.FirstName,
            Email = member.User != null ? member.User.Email! : member.Email,
            UserId = member.User != null ? member.UserId : null,
            MemberId = member.Id
        };
    }

}
