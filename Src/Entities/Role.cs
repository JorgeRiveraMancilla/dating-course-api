using Microsoft.AspNetCore.Identity;

namespace dating_course_api.Src.Entities
{
    public class Role : IdentityRole<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
