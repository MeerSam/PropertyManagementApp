using System;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class OwnerRepository(AppDbContext context) : IMemberRepository
{
    public async Task<IReadOnlyList<Member>> GetMembersAsync(string clientId)
    {
        var members = await context.Members
            .Where(m => m.ClientId ==clientId)
            // .Where(x =>  x.Id== clientId)
            .ToListAsync();
        return members;

    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        //we can avoid getting the error by using the update method
        // in case of identical savechanges which in SaveAllAsync method will return false.
        context.Entry(member).State = EntityState.Modified;
    }
}
