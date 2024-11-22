using AutoMapper;
using AutoMapper.QueryableExtensions;
using dating_course_api.Src.DTOs.Connection;
using dating_course_api.Src.DTOs.Group;
using dating_course_api.Src.DTOs.Message;
using dating_course_api.Src.Entities;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace dating_course_api.Src.Data.Repositories
{
    public class MessageRepository(DataContext dataContext, IMapper mapper) : IMessageRepository
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IMapper _mapper = mapper;

        public async Task CreateGroupAsync(CreateGroupDto createGroupDto)
        {
            var group = _mapper.Map<Group>(createGroupDto);
            _ = await _dataContext.Groups.AddAsync(group);
        }

        public async Task CreateMessageAsync(CreateMessageDto createMessageDto)
        {
            var message = _mapper.Map<Message>(createMessageDto);
            _ = await _dataContext.Messages.AddAsync(message);
        }

        public async Task DeleteMessageAsync(MessageDto messageDto)
        {
            var message =
                await _dataContext.Messages.FindAsync(messageDto.Id)
                ?? throw new Exception("Message not found");
            _ = _dataContext.Messages.Remove(message);
        }

        public async Task<ConnectionDto?> GetConnectionAsync(string connectionId)
        {
            return await _dataContext
                .Connections.Where(c => c.ConnectionId == connectionId)
                .ProjectTo<ConnectionDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<GroupDto?> GetGroupForConnectionAsync(string connectionId)
        {
            return await _dataContext
                .Groups.Include(g => g.Connections)
                .Where(g => g.Connections.Any(c => c.ConnectionId == connectionId))
                .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();
        }

        public async Task<MessageDto?> GetMessageAsync(int id)
        {
            return await _dataContext
                .Messages.ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        public async Task<GroupDto?> GetMessageGroupAsync(string groupName)
        {
            return await _dataContext
                .Groups.Include(g => g.Connections)
                .ProjectTo<GroupDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync(g => g.Name == groupName);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUserAsync(
            MessageParams messageParams
        )
        {
            var query = _dataContext.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox"
                    => query.Where(x =>
                        x.Recipient.Id == messageParams.UserId && x.RecipientDeleted == false
                    ),
                "Outbox"
                    => query.Where(x =>
                        x.Sender.Id == messageParams.UserId && x.SenderDeleted == false
                    ),
                _
                    => query.Where(x =>
                        x.Recipient.Id == messageParams.UserId
                        && x.DateRead == null
                        && x.RecipientDeleted == false
                    )
            };

            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(
                messages,
                messageParams.PageNumber,
                messageParams.PageSize
            );
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThreadAsync(
            int currentUserId,
            int recipientUserId
        )
        {
            var query = _dataContext
                .Messages.Where(x =>
                    x.RecipientId == currentUserId
                        && x.RecipientDeleted == false
                        && x.SenderId == recipientUserId
                    || x.SenderId == currentUserId
                        && x.SenderDeleted == false
                        && x.RecipientId == recipientUserId
                )
                .OrderBy(x => x.MessageSent)
                .AsQueryable();

            var unreadMessages = query
                .Where(x => x.DateRead == null && x.RecipientId == currentUserId)
                .ToList();

            if (unreadMessages.Count != 0)
            {
                unreadMessages.ForEach(x => x.DateRead = DateTime.UtcNow);
            }

            return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task RemoveConnection(ConnectionDto connectionDto)
        {
            var connection =
                await _dataContext.Connections.SingleOrDefaultAsync(c =>
                    c.ConnectionId == connectionDto.ConnectionId
                ) ?? throw new Exception("Connection not found");

            _ = _dataContext.Connections.Remove(connection);
        }
    }
}
