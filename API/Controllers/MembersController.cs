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
    public class MembersController(IMemberRepository memberRepository, 
        IPropertyRepository propertyRepository,
        IPhotoService photoService) : BaseApiController
    {
        [HttpGet]//https://localhost:5001/api/members
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams)
        {
            memberParams.CurrentClientId = User.GetClientId();
            var members =await memberRepository.GetMembersAsync();
            return Ok(members);
        }

        [HttpGet("{id}")] //https://localhost:5001/api/members/bob-id
        public async Task<ActionResult<MemberDto>> GetMember(string id)
        {
            var member = await memberRepository.GetMemberAsync(id);
            if (member == null) return NotFound("Member Not found");
            return member.ToDto();
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
            member.FirstName = memberUpdateDto.FirstName ?? member.FirstName;
            member.LastName = memberUpdateDto.LastName ?? member.LastName;
            member.Email = memberUpdateDto.Email ?? member.Email;
            member.Description = memberUpdateDto.Description ?? member.Description;
            if (member.User != null)
            {
                member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;
            }
            memberRepository.Update(member); // optional

            if (await memberRepository.SaveAllAsync()) return NoContent();
            return BadRequest("Update could not be completed");
        }
        
        [HttpGet("{id}/photos")]//localhost:5001/api/members/bob-id 
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await memberRepository.GetPhotosForMemberAsync(id));

        } 

        [HttpPost("add-photo")]
        public async Task<ActionResult<Photo>> AddPhoto([FromForm] FormFile file)
        {
            //  file will not come through body but [FromForm] to tell our API controller where to go looking for
            var memberId = User.GetMemberId();

            var member = await memberRepository.GetMemberForUpdateAsync(memberId);

            if (member == null) return BadRequest("Could not find the member to add photos");

            var result = await photoService.UploadPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = memberId,
                IsApproved = true
            };

            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                if (member.User != null)
                {
                    member.User.ImageUrl = photo.Url;
                }
            }
            member.Photos.Add(photo);

            if (await memberRepository.SaveAllAsync()) return photo;

            return BadRequest("Problem adding Photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await memberRepository.GetMemberForUpdateAsync(User.GetMemberId());

            if (member == null) return BadRequest("Cannot get a member from token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("Cannot set this as main image");
            }

            member.ImageUrl = photo.Url;
            if (member.User != null)
            {
                member.User.ImageUrl = photo.Url;

            }


            if (await memberRepository.SaveAllAsync()) return NoContent(); // return nocontent since its a update

            return BadRequest("Problem setting main photo"); // in case if the save is unsuccesfull return bad req
        }

        [HttpDelete("delete-photo/{photoId}")]

        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await memberRepository.GetMemberForUpdateAsync(User.GetMemberId());

            if (member == null) return BadRequest("Could not get a member to delete  photos");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (member.ImageUrl == photo?.Url || photo == null)
            {
                return BadRequest("This photo cannot be deleted");
            }

            if (photo.PublicId != null)
            {
                // this exists in Cloudinary and we need to delete from here
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);

            }

            member.Photos.Remove(photo);
            if (await memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the photo"); // in case if the save is unsuccesfull return bad req
        } 
    } 
}