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
    }
}
