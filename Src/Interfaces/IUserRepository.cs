using dating_course_api.Src.DTOs.Account;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Helpers.Pagination;
using Microsoft.AspNetCore.Identity;

namespace dating_course_api.Src.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> CheckPasswordAsync(int userId, string password);
        Task<IdentityResult> CreateUserAsync(RegisterDto registerDto, string password);
        Task<MemberDto?> GetMemberByEmailAsync(string email, bool isCurrentUser);
        Task<MemberDto?> GetMemberByIdAsync(int id, bool isCurrentUser);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByPhotoIdAsync(int photoId);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        Task<IEnumerable<UserWithRole>> GetUsersWithRolesAsync();
        void UpdateUser(UpdateUserDto updateUserDto);
        Task<bool> UserExistsByEmailAsync(string email);
    }
}
