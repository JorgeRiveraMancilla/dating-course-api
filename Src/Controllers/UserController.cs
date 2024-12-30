using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.Photo;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    [Authorize]
    public class UserController(IUnitOfWork unitOfWork, IPhotoService photoService)
        : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPhotoService _photoService = photoService;

        [HttpGet]
        public async Task<ActionResult<PagedList<MemberDto>>> GetUsers(
            [FromQuery] UserParams userParams
        )
        {
            var userId = User.GetUserId();
            userParams.CurrentUserId = userId;

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(users);

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MemberDto>> GetUser([FromRoute] int id)
        {
            var userId = User.GetUserId();
            var isCurrentUser = userId == id;

            var user = await _unitOfWork.UserRepository.GetMemberByIdAsync(
                id,
                isCurrentUser,
                userId
            );

            if (user is null)
                return NotFound();

            return user;
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateUser(
            [FromRoute] int id,
            [FromBody] UpdateUserDto updateUserDto
        )
        {
            var userId = User.GetUserId();

            if (id != userId)
                return Unauthorized();

            await _unitOfWork.UserRepository.UpdateUserAsync(id, updateUserDto);

            if (await _unitOfWork.Complete())
                return NoContent();

            return BadRequest("Failed to update the user");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDto>> AddPhoto([FromForm] IFormFile file)
        {
            var userId = User.GetUserId();
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);

            if (user is null)
                return NotFound();

            var result = await _photoService.CreatePhotoAsync(file);

            if (result.Error is not null)
                return BadRequest(result.Error.Message);

            var createPhotoDto = new CreatePhotoDto
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                UserId = userId
            };

            await _unitOfWork.PhotoRepository.CreatePhotoAsync(createPhotoDto);

            if (await _unitOfWork.Complete())
            {
                var photo = await _unitOfWork.PhotoRepository.GetPhotoByPublicIdAsync(
                    createPhotoDto.PublicId
                );
                return Ok(photo);
            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoId:int}")]
        public async Task<ActionResult> SetMainPhoto([FromRoute] int photoId)
        {
            var userId = User.GetUserId();
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);

            if (user is null)
                return NotFound();

            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);
            if (photo is null)
                return NotFound();

            if (photo.UserId != userId)
                return BadRequest("You cannot set this photo as main");

            var mainPhoto = await _unitOfWork.PhotoRepository.GetMainPhotoByUserIdAsync(userId);

            if (mainPhoto is not null && mainPhoto.Id == photoId)
                return NoContent();

            if (mainPhoto is not null)
                await _unitOfWork.PhotoRepository.SetPhotoIsMainAsync(userId, mainPhoto.Id, false);

            await _unitOfWork.PhotoRepository.SetPhotoIsMainAsync(userId, photoId, true);

            if (await _unitOfWork.Complete())
                return NoContent();

            return BadRequest("Problem setting main photo");
        }

        [HttpDelete("delete-photo/{photoId:int}")]
        public async Task<ActionResult> DeletePhoto([FromRoute] int photoId)
        {
            var userId = User.GetUserId();
            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);

            if (user is null)
                return BadRequest("User not found");

            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);

            if (photo is null || photo.PublicId is null)
                return BadRequest("Photo not found");
            else if (photo.UserId != userId)
                return Unauthorized();
            else if (photo.IsMain)
                return BadRequest("You cannot delete your main photo");

            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error is not null)
                return BadRequest(result.Error.Message);

            await _unitOfWork.PhotoRepository.DeletePhotoAsync(photo.Id);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Problem deleting photo");
        }
    }
}
