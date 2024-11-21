namespace dating_course_api.Src.DTOs.Photo
{
    public class PhotoForApprovalDto
    {
        public int Id { get; set; }
        public required string Url { get; set; }
        public int UserId { get; set; }
        public bool IsApproved { get; set; }
    }
}
