namespace dating_course_api.Src.DTOs.Message
{
    public class CreateMessageDto
    {
        public required int RecipientUserId { get; set; }
        public required string Content { get; set; }
    }
}
