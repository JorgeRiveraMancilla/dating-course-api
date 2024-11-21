using AutoMapper;
using AutoMapper.QueryableExtensions;
using dating_course_api.Src.DTOs.Photo;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data.Repositories
{
    public class PhotoRepository(DataContext dataContext, IMapper mapper) : IPhotoRepository
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IMapper _mapper = mapper;

        public async Task CreatePhotoAsync(CreatePhotoDto createPhotoDto)
        {
            var photo = _mapper.Map<Photo>(createPhotoDto);
            await _dataContext.Photos.AddAsync(photo);
        }

        public async Task<PhotoDto?> GetMainPhotoByUserIdAsync(int userId)
        {
            return await _dataContext
                .Photos.Where(p => p.UserId == userId && p.IsMain)
                .ProjectTo<PhotoDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PhotoDto?> GetPhotoByIdAsync(int id)
        {
            return await _dataContext
                .Photos.IgnoreQueryFilters()
                .ProjectTo<PhotoDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync()
        {
            return await _dataContext
                .Photos.IgnoreQueryFilters()
                .Where(p => p.IsApproved == false)
                .Select(u => new PhotoForApprovalDto
                {
                    Id = u.Id,
                    UserId = u.User.Id,
                    Url = u.Url,
                    IsApproved = u.IsApproved
                })
                .ToListAsync();
        }

        public async Task RemovePhotoAsync(PhotoDto photoDto)
        {
            var photo =
                await _dataContext.Photos.FindAsync(photoDto.Id)
                ?? throw new Exception("Photo not found");

            _dataContext.Photos.Remove(photo);
        }

        public async Task SetPhotoIsMainAsync(int userId, int photoId, bool isMain)
        {
            var user =
                await _dataContext
                    .Users.Include(u => u.Photos)
                    .FirstOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception("User not found");

            var photo =
                user.Photos.FirstOrDefault(p => p.Id == photoId)
                ?? throw new Exception("Photo not found");

            if (!photo.IsApproved)
                throw new Exception("Photo is not approved");

            photo.IsMain = isMain;
        }
    }
}
