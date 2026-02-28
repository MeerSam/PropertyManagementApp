using System;
using API.Entities;

namespace API.Interfaces;

public interface IMemberRepository
{
     void Update(Member member); // method does not return anything

    Task<bool> SaveAllAsync();
    Task<IReadOnlyList<Member>> GetMembersAsync();
    Task<Member?>GetMemberAsync(string memberId); 

    

}
