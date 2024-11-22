using dating_course_api.Src.DTOs.Message;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Helpers.Pagination;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    [Authorize]
    public class MessageController(IUnitOfWork unitOfWork) : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(NewMessageDto newMessageDto)
        {
            var userId = User.GetUserId();

            if (userId == newMessageDto.RecipientUserId)
                return BadRequest("You cannot message yourself");

            var sender = await _unitOfWork.UserRepository.GetUserByIdAsync(userId);
            var recipient = await _unitOfWork.UserRepository.GetUserByIdAsync(
                newMessageDto.RecipientUserId
            );

            if (recipient == null || sender == null)
                return BadRequest("Cannot send message at this time");

            var message = new CreateMessageDto
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                SenderUserName = sender.UserName,
                RecipientUserName = recipient.UserName,
                Content = newMessageDto.Content,
                SenderDeleted = false,
                RecipientDeleted = false
            };

            await _unitOfWork.MessageRepository.CreateMessageAsync(message);

            if (await _unitOfWork.Complete())
                return Ok(message);

            return BadRequest("Failed to save message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser(
            [FromQuery] MessageParams messageParams
        )
        {
            var userId = User.GetUserId();
            messageParams.UserId = userId;

            var messages = await _unitOfWork.MessageRepository.GetMessagesForUserAsync(
                messageParams
            );

            Response.AddPaginationHeader(messages);

            return Ok(messages);
        }

        [HttpGet("thread/{id:int}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(
            [FromRoute] int id
        )
        {
            var userId = User.GetUserId();
            var messageThread = await _unitOfWork.MessageRepository.GetMessageThreadAsync(
                userId,
                id
            );

            return Ok(messageThread);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteMessage([FromRoute] int id)
        {
            var userId = User.GetUserId();

            var message = await _unitOfWork.MessageRepository.GetMessageAsync(id);

            if (message is null)
                return BadRequest("Cannot delete this message");

            if (message.SenderId != userId && message.RecipientId != userId)
                return Forbid();

            if (message.SenderId == userId)
                message.SenderDeleted = true;

            if (message.RecipientId == userId)
                message.RecipientDeleted = true;

            if (message is { SenderDeleted: true, RecipientDeleted: true })
                await _unitOfWork.MessageRepository.DeleteMessageAsync(message);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Problem deleting the message");
        }
    }
}
