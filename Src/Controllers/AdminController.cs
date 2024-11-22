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
    }
}
