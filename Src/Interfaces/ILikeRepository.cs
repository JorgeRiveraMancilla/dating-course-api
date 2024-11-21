using dating_course_api.Src.DTOs.Like;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.Helpers.Pagination;

namespace dating_course_api.Src.Interfaces
{
    public interface ILikeRepository
    {
        Task CreateLikeAsync(CreateLikeDto createLikeDto);
        Task DeleteLikeAsync(LikeDto likeDto);
        Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int currentUserId);
        Task<LikeDto?> GetUserLikeAsync(int sourceUserId, int targetUserId);
        Task<PagedList<MemberDto>> GetUserLikesAsync(LikeParams likeParams);
    }
}
