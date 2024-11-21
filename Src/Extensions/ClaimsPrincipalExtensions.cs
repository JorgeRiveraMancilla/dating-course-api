using System.Security.Claims;

namespace dating_course_api.Src.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            return int.Parse(
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? throw new Exception("User not found")
            );
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User not found");
        }
    }
}
