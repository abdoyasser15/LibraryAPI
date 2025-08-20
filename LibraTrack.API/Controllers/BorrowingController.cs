using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraTrack.API.Controllers
{
    [Authorize]
    public class BorrowingController : BaseApiController
    {
        private readonly IBorrowingService _borrowingService;

        public BorrowingController(IBorrowingService borrowingService)
        {
            _borrowingService = borrowingService;
        }
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> GetAllBorrowingsWithUserAsync()
        {
            var borrowings = await _borrowingService.GetAllBorrowingsWithUserAsync();
            if (borrowings is null || !borrowings.Any())
                return NotFound(new ApiResponse(404, "No borrowings found."));
            return Ok(borrowings);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BorrowingDto>> GetBorrowingByIdAsync(int id)
        {
            var borrowing = await _borrowingService.GetBorrowingByIdAsync(id);
            if (borrowing is null)
                return NotFound(new ApiResponse(404, "Borrowing not found."));
            return Ok(borrowing);
        }
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [Authorize(Roles ="Admin")]
        [HttpDelete]
        public async Task<ActionResult> DeleteBorrowingAsync(int id)
        {
            var borrowing = await _borrowingService.GetByIdAsync(id);
            if (borrowing is null)
                return NotFound(new ApiResponse(404, "Borrowing not found."));
            await _borrowingService.DeleteAsync(id);
            return Ok(new ApiResponse(200, "Borrowing deleted successfully."));
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpPost]
        public async Task<ActionResult<BorrowingDto>> AddBorrowingAsync(CreateBorrowingDto borrowingDto)
        {
            if (borrowingDto is null)
                return BadRequest(new ApiResponse(400, "Invalid borrowing data."));

            if (borrowingDto.DueDate <= borrowingDto.BorrowDate)
                return BadRequest(new ApiResponse(400, "Due date must be after borrow date."));

            var borrowing = await _borrowingService.AddBorrowingAsync(borrowingDto);
            return CreatedAtAction(nameof(GetBorrowingByIdAsync), new { id = borrowing.Id }, borrowing);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin,Librarian")]
        [HttpPut("{id}")]
        public async Task<ActionResult<BorrowingDto>> UpdateBorrowingAsync(int id, UpdateBorrowingDto dto)
        {
            if (dto is null)
                return BadRequest(new ApiResponse(400, "Invalid borrowing data."));
            var updatedBorrowing = await _borrowingService.UpdateBorrowingAsync(id, dto);
            if (updatedBorrowing is null)
                return NotFound(new ApiResponse(404, "Borrowing not found."));
            return Ok(updatedBorrowing);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("userBorrowings/{userId}")]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> GetBorrowingsByUserIdAsync(string userId)
        {
            var borrowings = await _borrowingService.GetBorrowingsByUserIdAsync(userId);
            if (borrowings is null || !borrowings.Any())
                return NotFound(new ApiResponse(404, "No borrowings found for this user."));
            return Ok(borrowings);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin,Librarian")]
        [HttpGet("Overdue")]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> GetOverdueBorrowingsAsync()
        {
            var currentDate = DateTime.UtcNow;
            var overdueBorrowings = await _borrowingService.GetBorrowingsOverdueAsync(currentDate);
            if (overdueBorrowings is null || !overdueBorrowings.Any())
                return NotFound(new ApiResponse(404, "No overdue borrowings found."));
            return Ok(overdueBorrowings);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("Active")]
        public async Task<ActionResult<IReadOnlyList<BorrowingDto>>> GetActiveBorrowingsAsync()
        {
            var currentDate = DateTime.UtcNow;
            var activeBorrowings = await _borrowingService.GetBorrowingsActiveAsync(currentDate);
            if (activeBorrowings is null || !activeBorrowings.Any())
                return NotFound(new ApiResponse(404, "No active borrowings found."));
            return Ok(activeBorrowings);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpPut("{id}/Return")]
        public async Task<ActionResult<BorrowingDto?>> ReturnBorrowingAsync(int id, DateTime ReturnDate)
        {
            var borrowing = await _borrowingService.ReturnBorrowingAsync(id, ReturnDate);
            if (borrowing is null)
                return NotFound(new ApiResponse(404, "No overdue borrowings found.")); 
            return Ok(borrowing);
        }
    }
}
