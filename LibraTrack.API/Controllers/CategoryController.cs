using AutoMapper;
using Library.Core.Entities;
using Library.Core.ServiceContract;
using LibraTrack.API.DTOs;
using LibraTrack.API.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LibraTrack.API.Controllers
{
    [ApiExplorerSettings(GroupName = "public-v1")]
    public class CategoryController : BaseApiController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Category>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllAsync();
            if (categories is null)
                return NotFound(new ApiResponse(404, "No categories found."));
            var data = _mapper.Map<IReadOnlyList<CategoryToReturnDto>>(categories);
            return Ok(data);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryToReturnDto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound(new ApiResponse(404, "Category not found."));
            var data = _mapper.Map<CategoryToReturnDto>(category);
            return Ok(data);
        }
        [Authorize(Roles = "Admin")]
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [HttpPost]
        public async Task<ActionResult<CategoryToReturnDto>> AddCategory([FromBody] CreateCategoryDto createCategory)
        {
            var category = _mapper.Map<Category>(createCategory);
            var existingCategory = await _categoryService.AddAsync(category);
            if (existingCategory is null)
                return BadRequest(new ApiResponse(400, "Failed to add category."));
            var data = _mapper.Map<CategoryToReturnDto>(existingCategory);
            return Ok(data);
        }
        [Authorize(Roles = "Admin")]
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [HttpPatch("{id}")]
        public async Task<ActionResult<CategoryToReturnDto>> UpdateCategory(int id, [FromBody] JsonPatchDocument<UpdateCategoryDto> updateCategory)
        {
            if (updateCategory is null)
                return BadRequest(new ApiResponse(400, "Invalid update request."));
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound(new ApiResponse(404, "Category not found."));
            var updateDto = _mapper.Map<UpdateCategoryDto>(category);
            updateCategory.ApplyTo(updateDto, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(400, "Invalid update data."));
            _mapper.Map(updateDto, category);
            await _categoryService.UpdateAsync(category);
            var result = _mapper.Map<CategoryToReturnDto>(category);
            return result;
        }
        [Authorize(Roles = "Admin")]
        [ApiExplorerSettings(GroupName = "admin-v1")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<CategoryToReturnDto>> DeleteCategory(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category is null)
                return NotFound(new ApiResponse(404, "Category not found."));
            var deletedCategory = await _categoryService.DeleteAsync(id);
            var data = _mapper.Map<CategoryToReturnDto>(deletedCategory);
            return Ok(data);
        }
    }
}
