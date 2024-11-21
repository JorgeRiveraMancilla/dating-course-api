using System.ComponentModel.DataAnnotations;
using dating_course_api.Src.Validations;

namespace dating_course_api.Src.DTOs.Account
{
    public class RegisterDto
    {
        [StringLength(100, MinimumLength = 3)]
        public required string Name { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public required string KnownAs { get; set; }

        public required string Gender { get; set; }

        public required DateOnly BirthDate { get; set; }

        public required string City { get; set; }

        public required string Country { get; set; }

        [StringLength(20, MinimumLength = 8)]
        public required string Password { get; set; }

        [MatchField("Password")]
        [StringLength(20, MinimumLength = 8)]
        public required string ConfirmPassword { get; set; }
    }
}
