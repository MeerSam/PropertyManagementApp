using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    
    public class MembersController(AppDbContext context) : BaseApiController
    {
        [HttpGet]//https://localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            var users = await context.Users.ToListAsync();
         
            return users;
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
