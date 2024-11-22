namespace dating_course_api.Src.DTOs.Photo
{
    public class PhotoForApprovalDto
    {
        public required int Id { get; set; }
        public required int UserId { get; set; }
        public required string Url { get; set; }
        public required bool IsApproved { get; set; }
    }
}
