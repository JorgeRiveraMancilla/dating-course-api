using dating_course_api.Src.DTOs.Connection;

namespace dating_course_api.Src.DTOs.Group
{
    public class GroupDto
    {
        public required string Name { get; set; }
        public required ICollection<ConnectionDto> Connections { get; set; }
    }
}
