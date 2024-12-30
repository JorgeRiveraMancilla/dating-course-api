using AutoMapper;
using dating_course_api.Src.DTOs.Connection;
using dating_course_api.Src.DTOs.Group;
using dating_course_api.Src.DTOs.Message;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace dating_course_api.Src.SignalR;

public class ChatHub(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IHubContext<PresenceHub> presenceHub,
    PresenceTracker presenceTracker
) : Hub
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly PresenceTracker _presenceTracker = presenceTracker;
    private readonly IHubContext<PresenceHub> _presenceHub = presenceHub;

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];

        if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new HubException("Cannot join group");

        var groupName = GetGroupName(Context.User.GetUserId().ToString(), otherUser!);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        var messages = await _unitOfWork.MessageRepository.GetMessageThreadAsync(
            Context.User.GetUserId(),
            int.Parse(otherUser!)
        );

        if (_unitOfWork.HasChanges())
            await _unitOfWork.Complete();

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var group = await RemoveFromMessageGroup();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(NewMessageDto newMessageDto)
    {
        var userId = Context.User?.GetUserId() ?? throw new Exception("Could not get user");

        if (userId == newMessageDto.RecipientUserId)
            throw new HubException("You cannot message yourself");

        var sender = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
        var recipient = await _unitOfWork.UserRepository.GetUserByIdAsync(
            newMessageDto.RecipientUserId
        );

        if (recipient == null || sender == null)
            throw new HubException("User not found");

        var createMessage = new CreateMessageDto
        {
            SenderId = sender.Id,
            SenderUserName = sender.UserName ?? throw new HubException("Sender username is null"),
            RecipientId = recipient.Id,
            RecipientUserName =
                recipient.UserName ?? throw new HubException("Recipient username is null"),
            Content = newMessageDto.Content,
            SenderDeleted = false,
            RecipientDeleted = false
        };

        var groupName = GetGroupName(sender.Id.ToString(), recipient.Id.ToString());
        var group = await _unitOfWork.MessageRepository.GetMessageGroupAsync(groupName);

        // Si el destinatario está en el grupo, marcar como leído
        if (group?.Connections.Any(x => x.UserId == recipient.Id) == true)
        {
            createMessage.DateRead = DateTime.UtcNow;
        }
        else
        {
            // Notificar al destinatario si no está en el grupo de chat
            var connections = await _presenceTracker.GetConnectionsForUser(recipient.Id);
            if (0 < connections.Count)
            {
                await _presenceHub
                    .Clients.Clients(connections)
                    .SendAsync(
                        "NewMessageReceived",
                        new { userId = sender.Id, knownAs = sender.KnownAs }
                    );
            }
        }

        await _unitOfWork.MessageRepository.CreateMessageAsync(createMessage);

        if (await _unitOfWork.Complete())
        {
            await Clients
                .Group(groupName)
                .SendAsync("NewMessage", mapper.Map<MessageDto>(createMessage));
        }
    }

    private async Task<GroupDto> AddToGroup(string groupName)
    {
        var userId = Context.User?.GetUserId() ?? throw new Exception("Cannot get user");
        var connection = new ConnectionDto { ConnectionId = Context.ConnectionId, UserId = userId };

        var group = await _unitOfWork.MessageRepository.GetMessageGroupAsync(groupName);

        group ??= await CreateNewGroup(groupName);

        await _unitOfWork.MessageRepository.CreateConnectionAsync(connection);

        if (!await _unitOfWork.Complete())
            throw new HubException("Failed to add to group");

        return group;
    }

    private async Task<GroupDto> CreateNewGroup(string groupName)
    {
        var createGroup = new CreateGroupDto { Name = groupName };
        await _unitOfWork.MessageRepository.CreateGroupAsync(createGroup);

        if (!await _unitOfWork.Complete())
            throw new HubException("Failed to create group");

        return await _unitOfWork.MessageRepository.GetMessageGroupAsync(groupName)
            ?? throw new HubException("Failed to get created group");
    }

    private async Task<GroupDto> RemoveFromMessageGroup()
    {
        var group = await _unitOfWork.MessageRepository.GetGroupForConnectionAsync(
            Context.ConnectionId
        );
        var connection = group?.Connections.FirstOrDefault(x =>
            x.ConnectionId == Context.ConnectionId
        );

        if (connection == null || group == null)
            throw new HubException("Failed to remove from group");

        await _unitOfWork.MessageRepository.RemoveConnectionAsync(connection);

        if (!await _unitOfWork.Complete())
            throw new HubException("Failed to remove from group");

        return group;
    }

    private static string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
