using dating_course_api.Src.DTOs.Connection;
using dating_course_api.Src.DTOs.Group;
using dating_course_api.Src.DTOs.Message;
using dating_course_api.Src.Helpers.Pagination;

namespace dating_course_api.Src.Interfaces
{
    public interface IMessageRepository
    {
        Task CreateGroupAsync(CreateGroupDto createGroupDto);
        Task CreateMessageAsync(CreateMessageDto createMessageDto);
        Task DeleteMessageAsync(MessageDto messageDto);
        Task<ConnectionDto?> GetConnectionAsync(string connectionId);
        Task<GroupDto?> GetGroupForConnectionAsync(string connectionId);
        Task<MessageDto?> GetMessageAsync(int id);
        Task<GroupDto?> GetMessageGroupAsync(string groupName);
        Task<PagedList<MessageDto>> GetMessagesForUserAsync(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThreadAsync(int currentUserId, int recipientUserId);
        Task RemoveConnection(ConnectionDto connectionDto);
    }
}
