using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Helpers.Pagination;

namespace dating_course_api.Src.Interfaces
{
    public interface IUserRepository
    {
        Task<MemberDto?> GetMemberByEmailAsync(string email, bool isCurrentUser);
        Task<MemberDto?> GetMemberByIdAsync(int id, bool isCurrentUser);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<UserDto?> GetUserByPhotoIdAsync(int photoId);
        Task<IEnumerable<UserDto>> GetUsersAsync();
        void Update(UpdateUserDto updateUserDto);
        Task<bool> UserExistsByEmailAsync(string email);
    }
}
