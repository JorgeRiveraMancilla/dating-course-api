using AutoMapper;
using AutoMapper.QueryableExtensions;
using dating_course_api.Src.DTOs.Account;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data.Repositories
{
    public class UserRepository(
        DataContext dataContext,
        UserManager<User> userManager,
        IMapper mapper
    ) : IUserRepository
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        public async Task<bool> CheckPasswordAsync(int userId, string password)
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new Exception("User not found");
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> CreateUserAsync(RegisterDto registerDto, string password)
        {
            var user = _mapper.Map<User>(registerDto);
            user.UserName = user.UserName?.ToLower();

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<MemberDto?> GetMemberByEmailAsync(string email, bool isCurrentUser)
        {
            var query = _dataContext
                .Users.Where(u => u.Email == email)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            return await query.SingleOrDefaultAsync();
        }

        public async Task<MemberDto?> GetMemberByIdAsync(int id, bool isCurrentUser)
        {
            var query = _dataContext
                .Users.Where(u => u.Id == id)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            return await query.SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _dataContext
                .Users.AsQueryable()
                .Where(u => u.Id != userParams.CurrentUserId);

            if (userParams.Gender is not null)
                query = query.Where(x => x.Gender == userParams.Gender);

            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => minDob <= x.BirthDate && x.BirthDate <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };

            return await PagedList<MemberDto>.CreateAsync(
                query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider),
                userParams.PageNumber,
                userParams.PageSize
            );
        }

        public async Task<UserDto?> GetUserByEmailAsync(string email)
        {
            return await _dataContext
                .Users.Include(u => u.Photos)
                .Where(u => u.Email == email)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            return await _dataContext
                .Users.Include(u => u.Photos)
                .Where(u => u.Id == id)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<UserDto?> GetUserByPhotoIdAsync(int photoId)
        {
            return await _dataContext
                .Users.Include(u => u.Photos)
                .IgnoreQueryFilters()
                .Where(u => u.Photos.Any(p => p.Id == photoId))
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return await _dataContext
                .Users.Include(u => u.Photos)
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public void Update(UpdateUserDto updateUserDto)
        {
            var user = _mapper.Map<User>(updateUserDto);
            _dataContext.Entry(user).State = EntityState.Modified;
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}
