namespace dating_course_api.Src.DTOs.Photo
{
    public class PhotoDto
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public bool IsMain { get; set; }
        public required string PublicId { get; set; }
        public bool IsApproved { get; set; } = false;
        public int UserId { get; set; }
    }
}
