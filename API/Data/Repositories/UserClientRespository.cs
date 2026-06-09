using System;
using API.DTOs;
using API.DTOs.Auth;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Services;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class UserClientRespository(AppDbContext context) : IUserClientRepository
{
    public async Task<UserClientAccess?> GetAccessByIdAsync(string clientId, string userId)
    {
        return await context.UserClientAccess
         .Where(uca => uca.ClientId == clientId && uca.UserId == userId && uca.IsActive)
         .FirstOrDefaultAsync();
    }

    public async Task<UserClientAccess?> GetDocumentAccessById(string clientId, string userId, Document document)
    {
        return await context.UserClientAccess
        .Where(uca => uca.ClientId == clientId
            && uca.UserId == userId
            && uca.IsActive
            && document.ClientId == clientId)
        .FirstOrDefaultAsync();
    }

    public async Task<UserForEditDto> GetUserForUpdate(string clientId, string userId)
    {
        var uca = await context.UserClientAccess
        .Include(uca => uca.User)
            .ThenInclude(u => u.Members)
        .Where(uca =>
            uca.ClientId == clientId &&
            uca.UserId == userId)
        .FirstOrDefaultAsync();

        if (uca == null)
            throw new UnauthorizedAccessException("User does not have access to this client.");

        // Convert AppUser → UserDto (use your existing mapper)
        if (uca.User == null)
            throw new UnauthorizedAccessException("User does not have exists");

        return uca.ToUserDto();

    }

    public async Task<IReadOnlyList<UserForEditDto>> GetUsers(string clientId)
    {
        return await context.UserClientAccess
        .Include(uca => uca.User)
            .ThenInclude(u => u.Members)
        .Select(UserClientExtensions.ToUserDtoProjection())
        .Where(uca => uca.ClientId == clientId && uca.Role != HoaRoles.Admin && uca.MemberId == null)
        .ToListAsync();

    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(UserClientAccess ucAcess)
    {
        //we can avoid getting the error by using the update method
        // in case of identical savechanges which in SaveAllAsync method will return false.
        context.Entry(ucAcess).State = EntityState.Modified;
    }
}
