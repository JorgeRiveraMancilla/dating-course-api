using AutoMapper;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    [Authorize]
    public class UserController(IUnitOfWork unitOfWork, IPhotoService photoService, IMapper mapper)
        : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IPhotoService _photoService = photoService;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers(
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

            var user = await _unitOfWork.UserRepository.GetMemberByIdAsync(id, isCurrentUser);

            if (user is null)
                return NotFound();

            return user;
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(
            [FromRoute] int id,
            [FromBody] MemberUpdateDto memberUpdateDto
        )
        {
            var userId = User.GetUserId();

            if (id != userId)
                return Unauthorized();

            var updateUserDto = _mapper.Map<UpdateUserDto>(memberUpdateDto);
            _unitOfWork.UserRepository.UpdateUser(updateUserDto);

            if (await unitOfWork.Complete())
                return NoContent();

            return BadRequest("Failed to update the user");
        }
    }
}
