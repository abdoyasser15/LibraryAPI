using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using LibraTrack.API.DTOs;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraTrack.API.Controllers
{
    public class FineController : BaseApiController
    {
        private readonly IFineService _fineService;

        public FineController(IFineService fineService)
        {
            _fineService = fineService;
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<FineDto>>> GetAllFinesAsync()
        {
            var fines = await _fineService.GetAllFinesAsync();
            if (fines is null || !fines.Any())
                return NotFound(new ApiResponse(404, "No fines found."));
            return Ok(fines);
        }
        [Authorize(Roles = "Member,Admin")]
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [HttpGet("{id}")]
        public async Task<ActionResult<FineDto?>> GetFineByIdAsync(int id)
        {
            var fine = await _fineService.GetFineByIdAsync(id);
            if (fine is null)
                return NotFound(new ApiResponse(404, $"No Fine By Id {id}"));
            return Ok(fine);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Member,Admin")]
        [HttpGet("borrowing/{id}")]
        public async Task<ActionResult<FineDto?>> GetFineByBorrowingId(int id)
        {
            var fine = await _fineService.GetFineByBorrowingId(id);
            if (fine is null)
                return NotFound(new ApiResponse(404, $"No Fine By BorrowingId {id}"));
            return Ok(fine);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Member,Admin")]
        [HttpGet("User/{id}")]
        public async Task<ActionResult<FineDto?>> GetFinesByUserIdAsync(string id)
        {
            var fines = await _fineService.GetFineByUserIdAsync(id);
            if (fines is null)
                return NotFound(new ApiResponse(404, $"No Fines By UserId {id}"));
            return Ok(fines);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFineById(int id)
        {
            var fine = await _fineService.GetFineByIdAsync(id);
            if (fine is null)
                return NotFound(new ApiResponse(404, "Fine Not Found"));
            await _fineService.DeleteAsync(id);
            return Ok(fine);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateFineById(int id, UpdateFineDto updateFineDto)
        {
            if (updateFineDto is null)
                return BadRequest(new ApiResponse(400, "Invalid Fine"));
            var fine = await _fineService.UpdateFineAsync(id, updateFineDto);
            if (!fine)
                return NotFound(new ApiResponse(404, "Fine Not Found"));
            return Ok(fine);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [Authorize(Roles = "Member")]
        [HttpPost("{id}/pay")]
        public async Task<ActionResult> PayFineById(int id)
        {
            var fine = await _fineService.PayFineAsync(id);
            if (!fine)
                return NotFound(new ApiResponse(404, "Fine Not Found or Already Paid"));
            return Ok(new ApiResponse(200, "Fine Paid Successfully"));
        }
    }
}
