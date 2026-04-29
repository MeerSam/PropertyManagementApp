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
        public async Task<ActionResult<IReadOnlyList<PropertyDto>>> GetCurrentProperties(string id)
        {
            return Ok(await propertyRepository.GetMemberCurrentPropertiesAsync(id));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            var memberId = User.GetMemberId();
            if (memberId == null) return BadRequest("Oops - no id found in token");
            var member = await memberRepository.GetMemberForUpdateAsync(memberId);

            if (member == null) return BadRequest("Oops - could not get member");

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.FirstName =  memberUpdateDto.FirstName  ??  member.FirstName;
            member.LastName =  memberUpdateDto.LastName  ??  member.LastName;
            member.Email =  memberUpdateDto.Email  ??  member.Email;
            member.Description =  memberUpdateDto.Description  ??  member.Description;
            if (member.User != null)
            {
                member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;
            }
            memberRepository.Update(member); // optional

            if(await memberRepository.SaveAllAsync()) return NoContent();  
            return BadRequest("Update could not be completed");
        }
    }
}
