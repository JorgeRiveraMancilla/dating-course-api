using dating_course_api.Src.DTOs.Photo;

namespace dating_course_api.Src.Interfaces
{
    public interface IPhotoRepository
    {
        Task CreatePhotoAsync(CreatePhotoDto createPhotoDto);
        Task<PhotoDto?> GetMainPhotoByUserIdAsync(int userId);
        Task<PhotoDto?> GetPhotoByIdAsync(int id);
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
        Task RemovePhotoAsync(PhotoDto photoDto);
        Task SetPhotoIsMainAsync(int userId, int photoId, bool isMain);
    }
}
