using CloudinaryDotNet.Actions;

namespace dating_course_api.Src.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> CreatePhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
