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
    [Authorize]
    public class MembersController(IMemberRepository memberRepository, IPropertyRepository propertyRepository) : BaseApiController
    {
        [HttpGet]//https://localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams)
        { 
            memberParams.CurrentClientId = User.GetClientId();
            return Ok(await memberRepository.GetMembersAsync());
        }
 
        [HttpGet("{id}")] //https://localhost:5001/api/members/bob-id
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberAsync(id);
            if (member == null) return NotFound();
            return member;
        }   
        [HttpGet("{id}/properties")]
        public async Task<ActionResult<IReadOnlyList<Property>>> GetCurrentProperties(string id)
        {
            return Ok(await propertyRepository.GetMemberCurrentPropertiesAsync(id));
        }
    }
}
