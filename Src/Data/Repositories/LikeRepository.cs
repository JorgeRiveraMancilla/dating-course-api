using AutoMapper;
using AutoMapper.QueryableExtensions;
using dating_course_api.Src.DTOs.Like;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data.Repositories
{
    public class LikeRepository(DataContext dataContext, IMapper mapper) : ILikeRepository
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IMapper _mapper = mapper;

        public async Task CreateLikeAsync(CreateLikeDto createLikeDto)
        {
            var like = _mapper.Map<Like>(createLikeDto);
            await _dataContext.Likes.AddAsync(like);
        }

        public async Task DeleteLikeAsync(LikeDto likeDto)
        {
            var like =
                await _dataContext.Likes.FirstOrDefaultAsync(l =>
                    l.SourceUserId == likeDto.SourceUserId && l.TargetUserId == likeDto.TargetUserId
                ) ?? throw new Exception("Like not found");

            _dataContext.Likes.Remove(like);
        }

        public async Task<IEnumerable<int>> GetCurrentUserLikeIdsAsync(int currentUserId)
        {
            return await _dataContext
                .Likes.Where(l => l.SourceUserId == currentUserId)
                .Select(l => l.TargetUserId)
                .ToListAsync();
        }

        public async Task<LikeDto?> GetUserLikeAsync(int sourceUserId, int targetUserId)
        {
            return await _dataContext
                .Likes.Where(l => l.SourceUserId == sourceUserId && l.TargetUserId == targetUserId)
                .Select(l => new LikeDto
                {
                    SourceUserId = l.SourceUserId,
                    TargetUserId = l.TargetUserId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetUserLikesAsync(LikeParams likeParams)
        {
            var likes = _dataContext.Likes.AsQueryable();
            IQueryable<MemberDto> query;

            switch (likeParams.Predicate)
            {
                case "liked":
                    query = likes
                        .Where(x => x.SourceUserId == likeParams.UserId)
                        .Select(x => x.TargetUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;
                case "likedBy":
                    query = likes
                        .Where(x => x.TargetUserId == likeParams.UserId)
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;
                default:
                    var likeIds = await GetCurrentUserLikeIdsAsync(likeParams.UserId);

                    query = likes
                        .Where(x =>
                            x.TargetUserId == likeParams.UserId && likeIds.Contains(x.SourceUserId)
                        )
                        .Select(x => x.SourceUser)
                        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                    break;
            }

            return await PagedList<MemberDto>.CreateAsync(
                query,
                likeParams.PageNumber,
                likeParams.PageSize
            );
        }
    }
}
