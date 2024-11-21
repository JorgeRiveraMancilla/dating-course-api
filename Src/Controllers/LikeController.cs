using dating_course_api.Src.DTOs.Like;
using dating_course_api.Src.Extensions;
using dating_course_api.Src.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dating_course_api.Src.Controllers
{
    public class LikeController(IUnitOfWork unitOfWork) : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike([FromRoute] int targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if (sourceUserId == targetUserId)
                return BadRequest("You cannot like yourself");

            var existingLike = await _unitOfWork.LikeRepository.GetUserLikeAsync(
                sourceUserId,
                targetUserId
            );

            if (existingLike is null)
            {
                var createLikeDto = new CreateLikeDto
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };

                await _unitOfWork.LikeRepository.CreateLikeAsync(createLikeDto);
            }
            else
                await _unitOfWork.LikeRepository.DeleteLikeAsync(existingLike);

            if (await _unitOfWork.Complete())
                return Ok();

            return BadRequest("Failed to update like");
        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            var userId = User.GetUserId();
            var likeIds = await _unitOfWork.LikeRepository.GetCurrentUserLikeIdsAsync(userId);

            return Ok(likeIds);
        }
    }
}
