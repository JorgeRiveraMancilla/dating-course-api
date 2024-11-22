using dating_course_api.Src.DTOs.Photo;

namespace dating_course_api.Src.DTOs.User
{
    public class UserDto
    {
        public required int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required string KnownAs { get; set; }
        public required string Gender { get; set; }
        public string? Introduction { get; set; }
        public string? Interests { get; set; }
        public string? LookingFor { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public required DateTime Created { get; set; }
        public required DateTime LastActive { get; set; }
        public List<PhotoDto> Photos { get; set; } = [];
    }
}
