using AutoMapper;
using AutoMapper.QueryableExtensions;
using dating_course_api.Src.DTOs.Account;
using dating_course_api.Src.DTOs.Member;
using dating_course_api.Src.DTOs.User;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using dating_course_api.Src.Validations;
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

        public async Task<IdentityResult> AddRolesToUserAsync(int userId, string[] roles)
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new Exception("User not found");

            return await _userManager.AddToRolesAsync(user, roles);
        }

        public async Task<IdentityResult> ChangePasswordAsync(
            int userId,
            string CurrentPassword,
            string NewPassword
        )
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new Exception("User not found");

            return await _userManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
        }

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

            _userManager.Options.User.RequireUniqueEmail = true;
            _userManager.UserValidators.Clear();
            _userManager.UserValidators.Add(new CustomUserValidator());

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

        public async Task<MemberDto?> GetMemberByIdAsync(
            int id,
            bool isCurrentUser,
            int? currentUserId = null
        )
        {
            var query = _dataContext.Users.Where(u => u.Id == id).AsQueryable();

            if (isCurrentUser)
                query = query.IgnoreQueryFilters();

            var likeExists =
                currentUserId.HasValue
                && await _dataContext.Likes.AnyAsync(l =>
                    l.SourceUserId == currentUserId && l.TargetUserId == id
                );

            var projectedQuery = query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .Select(member => new MemberDto
                {
                    Id = member.Id,
                    UserName = member.UserName,
                    Email = member.Email,
                    KnownAs = member.KnownAs,
                    BirthDate = member.BirthDate,
                    Age = member.Age,
                    Gender = member.Gender,
                    Introduction = member.Introduction,
                    LookingFor = member.LookingFor,
                    Interests = member.Interests,
                    City = member.City,
                    Country = member.Country,
                    Created = member.Created,
                    LastActive = member.LastActive,
                    Photos = member.Photos,
                    MainPhoto = member.MainPhoto,
                    IsLiked = likeExists
                });

            return await projectedQuery.SingleOrDefaultAsync();
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

            var likedUserIds = await _dataContext
                .Likes.Where(l => l.SourceUserId == userParams.CurrentUserId)
                .Select(l => l.TargetUserId)
                .ToListAsync();

            var projectedQuery = query
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .Select(member => new MemberDto
                {
                    Id = member.Id,
                    UserName = member.UserName,
                    Email = member.Email,
                    KnownAs = member.KnownAs,
                    Age = member.Age,
                    Gender = member.Gender,
                    Introduction = member.Introduction,
                    LookingFor = member.LookingFor,
                    Interests = member.Interests,
                    City = member.City,
                    Country = member.Country,
                    Created = member.Created,
                    LastActive = member.LastActive,
                    Photos = member.Photos,
                    MainPhoto = member.MainPhoto,
                    IsLiked = likedUserIds.Contains(member.Id)
                });

            return await PagedList<MemberDto>.CreateAsync(
                projectedQuery,
                userParams.PageNumber,
                userParams.PageSize
            );
        }

        public async Task<IEnumerable<string?>> GetRoleNamesAsync()
        {
            return await _dataContext.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRolesFromUserAsync(int userId)
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new Exception("User not found");

            return await _userManager.GetRolesAsync(user);
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

        public async Task<PagedList<UserWithRole>> GetUsersWithRolesAsync(
            PaginationParams paginationParams
        )
        {
            var query = _dataContext
                .Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ProjectTo<UserWithRole>(_mapper.ConfigurationProvider)
                .AsNoTracking();

            return await PagedList<UserWithRole>.CreateAsync(
                query,
                paginationParams.PageNumber,
                paginationParams.PageSize
            );
        }

        public async Task<IdentityResult> RemoveRolesFromUserAsync(int userId, string[] roles)
        {
            var user =
                await _userManager.FindByIdAsync(userId.ToString())
                ?? throw new Exception("User not found");

            return await _userManager.RemoveFromRolesAsync(user, roles);
        }

        public async Task UpdateUserAsync(int userid, UpdateUserDto updateUserDto)
        {
            var user =
                await _dataContext.Users.FindAsync(userid) ?? throw new Exception("User not found");

            _mapper.Map(updateUserDto, user);
        }

        public async Task<bool> UserExistsByEmailAsync(string email)
        {
            return await _dataContext.Users.AnyAsync(u => u.Email == email);
        }
    }
}
