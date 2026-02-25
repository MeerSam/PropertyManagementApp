using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

    public class MembersController(AppDbContext context, IMemberRepository owner) : BaseApiController
    {
        [HttpGet]//https://localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams)
        {
            // var users = await context.Users.ToListAsync();
            memberParams.CurrentClientId = User.GetClientId();
            return Ok(await owner.GetMembersAsync(memberParams.CurrentClientId));
        }

        [Authorize]
        [HttpGet("{id}")] //https://localhost:5001/api/members/bob-id
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            var user = await context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return user;
        }
        // [HttpGet("clients")]
        // public async Task<ActionResult<IReadOnlyList<Client>>> GetClientsForCurrentUser(string id)
        // { 
        //     var user = await context.Users.FirstOrDefaultAsync("id");
        //     if (user == null) return NotFound();

        //     var clients = await context.Members
        //     .Where(m => m.UserId == userId)
        //     .Select(m => m.Client)
        //     .ToListAsync(); 


        //     return clients;

        // }
    }
}
