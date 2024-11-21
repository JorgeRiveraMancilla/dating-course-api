namespace dating_course_api.Src.DTOs.Account
{
    public class AuthDto
    {
        public required int UserId { get; set; }
        public required string UserName { get; set; }
        public required string Token { get; set; }
    }
}
