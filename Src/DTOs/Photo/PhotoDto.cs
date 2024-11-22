namespace dating_course_api.Src.DTOs.Photo
{
    public class PhotoDto
    {
        public required int Id { get; set; }
        public required string Url { get; set; }
        public required bool IsMain { get; set; }
        public required bool IsApproved { get; set; }
        public required string PublicId { get; set; }
        public required int UserId { get; set; }
    }
}
