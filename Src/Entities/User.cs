using Microsoft.AspNetCore.Identity;

namespace dating_course_api.Src.Entities
{
    public class User : IdentityUser<int>
    {
        public required DateOnly BirthDate { get; set; }
        public required string KnownAs { get; set; }
        public required string Gender { get; set; }
        public string? Introduction { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public List<Photo> Photos { get; set; } = [];
        public List<Like> LikedByUsers { get; set; } = [];
        public List<Like> LikedUsers { get; set; } = [];
        public List<Message> MessagesSent { get; set; } = [];
        public List<Message> MessagesReceived { get; set; } = [];
        public ICollection<UserRole> UserRoles { get; set; } = [];

        public User()
        {
            SecurityStamp = Guid.NewGuid().ToString();
        }
    }
}
