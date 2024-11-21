namespace dating_course_api.Src.Helpers.Pagination
{
    public class UserParams : PaginationParams
    {
        public string? Gender { get; set; }
        public int? CurrentUserId { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 100;
        public string OrderBy { get; set; } = "lastActive";
    }
}
