namespace dating_course_api.Src.Helpers.Pagination
{
    public class LikeParams : PaginationParams
    {
        public int UserId { get; set; }
        public required string Predicate { get; set; } = "liked";
    }
}
