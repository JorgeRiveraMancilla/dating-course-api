using System.ComponentModel.DataAnnotations;

namespace dating_course_api.Src.DTOs.Account
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }

        [StringLength(20, MinimumLength = 8)]
        public required string Password { get; set; }
    }
}
