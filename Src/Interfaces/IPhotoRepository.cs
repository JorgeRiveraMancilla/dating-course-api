using dating_course_api.Src.DTOs.Photo;

namespace dating_course_api.Src.Interfaces
{
    public interface IPhotoRepository
    {
        Task ApprovePhotoAsync(int photoId);
        Task CreatePhotoAsync(CreatePhotoDto createPhotoDto);
        Task DeletePhotoAsync(int photoId);
        Task<PhotoDto?> GetMainPhotoByUserIdAsync(int userId);
        Task<PhotoDto?> GetPhotoByIdAsync(int id);
        Task<PhotoDto?> GetPhotoByPublicIdAsync(string publicId);
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
        Task SetPhotoIsMainAsync(int userId, int photoId, bool isMain);
    }
}
