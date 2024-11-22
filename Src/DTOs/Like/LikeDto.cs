namespace dating_course_api.Src.DTOs.Like
{
    public class LikeDto
    {
        public required int SourceUserId { get; set; }
        public required int TargetUserId { get; set; }
    }
}
