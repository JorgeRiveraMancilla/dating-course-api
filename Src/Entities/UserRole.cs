using Microsoft.AspNetCore.Identity;

namespace dating_course_api.Src.Entities
{
    public class UserRole : IdentityUserRole<int>
    {
        public required User User { get; set; }
        public required Role Role { get; set; }
    }
}
