using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraTrack.API.Controllers
{    
    [ApiController]
    public class RatingsController : BaseApiController
    {
        private readonly IRatingsService _ratingsService;
        public RatingsController(IRatingsService ratingsService)
        {
            _ratingsService = ratingsService;
        }
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<Ratings>> GetAllRatingsAsync()
        {
            var ratings = await _ratingsService.GetAllRatingsAsync();
            if (ratings == null || !ratings.Any())
                return NotFound(new ApiResponse(404, "No ratings found."));
            return Ok(ratings);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin,Member")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RatingDto>> GetRatingByIdAsync(int id)
        {
            var rating = await _ratingsService.GetRatingByIdAsync(id);
            if (rating == null)
                return NotFound(new ApiResponse(404, "Rating not found."));
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && rating.UserId != userId)
                return Forbid();
            return Ok(rating);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("book/{bookId}")]
        public async Task<ActionResult<IReadOnlyList<RatingDto>>> GetRatingByBookId(int bookId)
        {
            var rating = await _ratingsService.GetRatingByBookId(bookId);
            if (rating is null || !rating.Any())
                return NotFound(new ApiResponse(404, "Rating for the specified book not found."));
            return Ok(rating);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [Authorize(Roles = "Member")]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateRatingAsync(int id, [FromBody] RatingDto rating)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != rating.UserId)
                return BadRequest(new ApiResponse(400, "You can only update your own ratings."));
            if (id!=rating.Id)
                return BadRequest(new ApiResponse(400, "Rating ID mismatch."));
            if (rating is null)
                return BadRequest(new ApiResponse(400, "Invalid rating data."));
            
            var result = await _ratingsService.UpdateRatingAsync(id, rating);
            if (!result)
                return NotFound(new ApiResponse(404, "Rating not found."));
            
            return Ok(new ApiResponse(200,"Rating Updated Successfully"));
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Member,Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteRatingAsync(int id)
        {
            var rating = await _ratingsService.GetRatingByIdAsync(id);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId!=rating?.UserId)
                return BadRequest(new ApiResponse(400, "You can only delete your own ratings."));
            var result = await _ratingsService.DeleteRatingAsync(id);
            if (!result)
                return NotFound(new ApiResponse(404, "Rating not found."));
            
            return Ok(new ApiResponse(200, "Rating deleted successfully."));
        }
    }
}
