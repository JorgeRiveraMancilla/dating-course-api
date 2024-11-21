namespace dating_course_api.Src.DTOs.Photo
{
    public class CreatePhotoDto
    {
        public required string Url { get; set; }
        public required string PublicId { get; set; }
        public required int UserId { get; set; }
    }
}
