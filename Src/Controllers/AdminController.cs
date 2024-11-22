using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    public class AdminController(IUnitOfWork unitOfWork, IPhotoService photoService)
        : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPhotoService _photoService = photoService;

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _unitOfWork.UserRepository.GetUsersWithRolesAsync();

            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{id:int}")]
        public async Task<ActionResult> EditRoles([FromRoute] int id, [FromQuery] string roles)
        {
            if (string.IsNullOrEmpty(roles))
                return BadRequest("you must select at least one role");

            var nameRoles = await _unitOfWork.UserRepository.GetRoleNamesAsync();
            var selectedRoles = roles.Split(",").ToArray();

            if (selectedRoles.Any(role => !nameRoles.Contains(role)))
                return BadRequest("Invalid roles");

            var user = await _unitOfWork.UserRepository.GetUserByIdAsync(id);

            if (user is null)
                return BadRequest("User not found");

            var userId = user.Id;
            var userRoles = await _unitOfWork.UserRepository.GetRolesFromUserAsync(userId);

            var result = await _unitOfWork.UserRepository.AddRolesToUserAsync(
                userId,
                selectedRoles.Except(userRoles).ToArray()
            );

            if (!result.Succeeded)
                return BadRequest("Failed to add roles");

            result = await _unitOfWork.UserRepository.RemoveRolesFromUserAsync(
                userId,
                userRoles.Except(selectedRoles).ToArray()
            );

            if (!result.Succeeded)
                return BadRequest("Failed to remove roles");

            return Ok();
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult> GetPhotosForModeration()
        {
            var photos = await _unitOfWork.PhotoRepository.GetUnapprovedPhotosAsync();

            return Ok(photos);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("approve-photo/{photoId}")]
        public async Task<ActionResult> ApprovePhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);

            if (photo is null)
                return BadRequest("Could not get photo from db");

            await _unitOfWork.PhotoRepository.ApprovePhotoAsync(photoId);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Failed to approve photo");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpPost("reject-photo/{photoId}")]
        public async Task<ActionResult> RejectPhoto(int photoId)
        {
            var photo = await _unitOfWork.PhotoRepository.GetPhotoByIdAsync(photoId);

            if (photo is null)
                return BadRequest("Could not get photo from db");

            if (photo.PublicId is not null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if (result.Result == "ok")
                    await _unitOfWork.PhotoRepository.DelePhotoAsync(photoId);
            }
            else
                await _unitOfWork.PhotoRepository.DelePhotoAsync(photoId);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Failed to reject photo");
        }
    }
}
