namespace dating_course_api.Src.DTOs.Message
{
    public class CreateMessageDto
    {
        public required int SenderId { get; set; }
        public required string SenderUserName { get; set; }
        public required int RecipientId { get; set; }
        public required string RecipientUserName { get; set; }
        public required string Content { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public required bool SenderDeleted { get; set; }
        public required bool RecipientDeleted { get; set; }
    }
}
