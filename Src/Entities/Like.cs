namespace dating_course_api.Src.Entities
{
    public class Like
    {
        public required User SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public required User TargetUser { get; set; }
        public int TargetUserId { get; set; }
    }
}
