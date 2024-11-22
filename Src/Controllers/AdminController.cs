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
    }
}
