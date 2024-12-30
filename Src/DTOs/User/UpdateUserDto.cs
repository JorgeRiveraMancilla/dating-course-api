using System.ComponentModel.DataAnnotations;

namespace dating_course_api.Src.DTOs.User
{
    public class UpdateUserDto
    {
        [StringLength(100, MinimumLength = 3)]
        public required string UserName { get; set; }

        [StringLength(100, MinimumLength = 3)]
        public required string KnownAs { get; set; }
        public required string Gender { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
    }
}
