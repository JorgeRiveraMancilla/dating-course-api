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
                return BadRequest(new { error = "Debes ser mayor de edad para registrarte." });

            var exists = await _unitOfWork.UserRepository.UserExistsByEmailAsync(registerDto.Email);
            if (exists)
                return Conflict(new { error = "El correo electrónico ya está en uso." });

            var result = await _unitOfWork.UserRepository.CreateUserAsync(
                registerDto,
                registerDto.Password
            );
            if (!result.Succeeded)
                return BadRequest(
                    new { error = result.Errors.Select(e => e.Description).ToList() }
                );

            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthDto>> Login([FromBody] LoginDto loginDto)
        {
            var user = await _unitOfWork.UserRepository.GetUserByEmailAsync(loginDto.Email);
            if (user is null)
                return Unauthorized(new { error = "Credenciales inválidas." });

            var result = await _unitOfWork.UserRepository.CheckPasswordAsync(
                user.Id,
                loginDto.Password
            );
            if (!result)
                return Unauthorized(new { error = "Credenciales inválidas." });

            var token = await _tokenService.CreateTokenAsync(user.Id, user.UserName);
            var auth = new AuthDto
            {
                UserId = user.Id,
                UserName = user.UserName,
                UserGender = user.Gender,
                Token = token
            };

            return Ok(auth);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword(
            [FromBody] ChangePasswordDto changePasswordDto
        )
        {
            var userId = User.GetUserId();

            var result = await _unitOfWork.UserRepository.ChangePasswordAsync(
                userId,
                changePasswordDto.CurrentPassword,
                changePasswordDto.NewPassword
            );

            if (!result.Succeeded)
                return BadRequest(
                    new { error = result.Errors.Select(e => e.Description).ToList() }
                );

            return Ok();
        }
    }
}
