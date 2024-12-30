namespace dating_course_api.Src.Entities
{
    public class Connection
    {
        public required string ConnectionId { get; set; }
        public required int UserId { get; set; }
        public required bool Connected { get; set; }
        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;
    }
}
