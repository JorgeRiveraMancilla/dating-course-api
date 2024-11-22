namespace dating_course_api.Src.DTOs.Message
{
    public class NewMessageDto
    {
        public required int RecipientUserId { get; set; }
        public required string Content { get; set; }
    }
}
