using dating_course_api.Src.DTOs.Account;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    public class AccountController(IUserRepository userRepository, ITokenService tokenService)
        : BaseApiController
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ITokenService _tokenService = tokenService;

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto registerDto)
        {
            if (registerDto.BirthDate.CalculateAge() < 18)
                return BadRequest("You must be at least 18 years old to register");

            var exists = await _userRepository.UserExistsByEmailAsync(registerDto.Email);
            if (exists)
                return Conflict("Email is already in use");

            var result = await _userRepository.CreateUserAsync(registerDto, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok();
        }
    }
}
