using dating_course_api.Src.DTOs.Account;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    public class AccountController(IUnitOfWork unitOfWork, ITokenService tokenService)
        : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto.BirthDate.CalculateAge() < 18)
                return BadRequest("You must be at least 18 years old to register");

            var exists = await _unitOfWork.UserRepository.UserExistsByEmailAsync(registerDto.Email);
            if (exists)
                return Conflict("Email is already in use");

            var result = await _unitOfWork.UserRepository.CreateUserAsync(
                registerDto,
                registerDto.Password
            );
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginDto.Email);
            if (user is null)
                return Unauthorized("Invalid credentials");

            var result = await _unitOfWork.UserRepository.CheckPasswordAsync(
                user.Id,
                loginDto.Password
            );
            if (!result)
                return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(user.Id, user.UserName);
            var auth = new AuthDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                Token = token
            };

            return Ok(auth);
        }
    }
}
