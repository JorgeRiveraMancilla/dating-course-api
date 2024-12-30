using System.ComponentModel.DataAnnotations;
using dating_course_api.Src.Validations;

namespace dating_course_api.Src.DTOs.Account
{
    public class ChangePasswordDto
    {
        [StringLength(20, MinimumLength = 8)]
        public required string CurrentPassword { get; set; }

        [StringLength(20, MinimumLength = 8)]
        public required string NewPassword { get; set; }

        [MatchField("NewPassword")]
        [StringLength(20, MinimumLength = 8)]
        public required string ConfirmPassword { get; set; }
    }
}
