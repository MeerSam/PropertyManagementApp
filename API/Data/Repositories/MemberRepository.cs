using System;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories;

public class MemberRepository(AppDbContext context, ITenantService tenantService) : IMemberRepository
{
    public async Task<Member?> GetMemberAsync(string memberId)
    {
        var member = await context.Members.FindAsync(memberId);

        if (member == null) return member;

        if (member.ClientId != tenantService.GetCurrentClientId())
        {
            throw new UnauthorizedAccessException("Cannot access this member");
        }
        return member;
    }

  

    public async  Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        var members = await context.Members
            .Include(m => m.PropertyOwnerships) 
            .Where(m => m.ClientId == tenantService.GetCurrentClientId()) 
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
