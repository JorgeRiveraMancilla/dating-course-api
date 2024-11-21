using dating_course_api.Src.DTOs.Photo;

namespace dating_course_api.Src.Interfaces
{
    public interface IPhotoRepository
    {
        Task CreatePhotoAsync(CreatePhotoDto createPhotoDto);
        Task<PhotoDto?> GetPhotoByIdAsync(int id);
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotosAsync();
        Task RemovePhotoAsync(PhotoDto photoDto);
    }
}
