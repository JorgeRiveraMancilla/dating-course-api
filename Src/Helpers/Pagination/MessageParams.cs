namespace dating_course_api.Src.Helpers.Pagination
{
    public class MessageParams : PaginationParams
    {
        public int? UserId { get; set; }
        public string Container { get; set; } = "Unread";
    }
}
