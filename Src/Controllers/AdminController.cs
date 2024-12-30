using dating_course_api.Src.DTOs.Photo;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Helpers.Pagination;
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
        public async Task<ActionResult<PagedList<UserWithRole>>> GetUsersWithRoles(
            [FromQuery] PaginationParams paginationParams
        )
        {
            var users = await _unitOfWork.UserRepository.GetUsersWithRolesAsync(paginationParams);
            Response.AddPaginationHeader(users);
            return Ok(users);
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{userId:int}")]
        public async Task<ActionResult<IList<string>>> EditRoles(
            [FromRoute] int userId,
            [FromQuery] string roles
        )
        {
            var targetUser = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            if (targetUser is null)
                return NotFound("Usuario no encontrado");

            var targetUserRoles = await _unitOfWork.UserRepository.GetRolesFromUserAsync(
                targetUser.Id
            );
            if (targetUserRoles.Contains("Admin"))
                return BadRequest("No se pueden editar los roles de un administrador");

            if (string.IsNullOrEmpty(roles))
                return BadRequest("Debe seleccionar al menos un rol");

            var selectedRoles = roles.Split(",").ToArray();
            var validRoles = new[] { "Member", "Moderator" };

            if (selectedRoles.Any(role => !validRoles.Contains(role)))
                return BadRequest("Roles inválidos");

            if (!selectedRoles.Contains("Member"))
                return BadRequest("El usuario debe tener el rol Member");

            if (selectedRoles.Except(validRoles).Any())
                return BadRequest("Solo se permiten los roles Member y Moderator");

            var result = await _unitOfWork.UserRepository.AddRolesToUserAsync(
                targetUser.Id,
                selectedRoles.Except(targetUserRoles).ToArray()
            );

            if (!result.Succeeded)
                return BadRequest("Error al agregar roles");

            result = await _unitOfWork.UserRepository.RemoveRolesFromUserAsync(
                targetUser.Id,
                targetUserRoles.Except(selectedRoles).ToArray()
            );

            if (!result.Succeeded)
                return BadRequest("Error al eliminar roles");

            var updatedRoles = await _unitOfWork.UserRepository.GetRolesFromUserAsync(
                targetUser.Id
            );
            return Ok(updatedRoles);
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public async Task<ActionResult<PhotoForApprovalDto>> GetPhotosForModeration()
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
                    await _unitOfWork.PhotoRepository.DeletePhotoAsync(photoId);
            }
            else
                await _unitOfWork.PhotoRepository.DeletePhotoAsync(photoId);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Failed to reject photo");
        }
    }
}
