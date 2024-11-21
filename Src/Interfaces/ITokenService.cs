namespace dating_course_api.Src.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(int userId, string userEmail);
    }
}
