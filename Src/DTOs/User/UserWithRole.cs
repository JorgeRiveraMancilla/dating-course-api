namespace dating_course_api.Src.DTOs.User
{
    public class UserWithRole
    {
        public required int Id { get; set; }
        public required string UserName { get; set; }
        public required string[] Roles { get; set; }
    }
}
