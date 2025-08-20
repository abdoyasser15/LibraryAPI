using AutoMapper;
using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using Library.Core.Specifications.BookSpecs;
using LibraTrack.API.DTOs;
using LibraTrack.API.Errors;
using LibraTrack.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraTrack.API.Controllers
{
    [Authorize]
    public class BookController : BaseApiController
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;
        public BookController(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [Cashed]
        [HttpGet]
        public async Task<ActionResult<Pagination<BookToReturnDto>>> GetAllBooks([FromQuery] BookSpecificationParams specParam)
        {
            var books = await _bookService.GetAllBooksAsync(specParam);
            if (books is null)
                return NotFound(new ApiResponse(404));
            var data = _mapper.Map<IReadOnlyList<BookToReturnDto>>(books);
            var count = await _bookService.GetCountAsync(specParam);
            return Ok(new Pagination<BookToReturnDto>(specParam.PageIndex, specParam.PageSize, count, data));
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BookToReturnDto>> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book is null)
                return NotFound(new ApiResponse(404));
            var data = _mapper.Map<BookToReturnDto>(book);
            return Ok(data);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin, Librarian")]
        [HttpPost]
        public async Task<ActionResult<BookToReturnDto>> AddBook([FromBody] CreateBookDto Book)
        {
            if (Book.TotalCopies < Book.AvalibleCopies)
                return BadRequest(new ApiResponse(400, "Total copies cannot be less than available copies."));
            var book = _mapper.Map<Book>(Book);
            var existingBook = await _bookService.AddBookAsync(book);
            var spec = new BookWithCategorySpecification(existingBook.Id);
            existingBook = await _bookService.GetBookByIdAsync(existingBook.Id);
            var data = _mapper.Map<BookToReturnDto>(existingBook);
            return Ok(data);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin, Librarian")]
        [HttpPut("{id}")]
        public async Task<ActionResult<BookToReturnDto>> UpdateBook(int id, [FromBody] UpdateBookDto updateDto)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book is null)
                return NotFound(new ApiResponse(404));
            _mapper.Map(updateDto, book);
            await _bookService.UpdateBookAsync(book);
            var updatedBook = await _bookService.GetBookByIdAsync(id);
            var bookToReturn = _mapper.Map<BookToReturnDto>(updatedBook);
            return Ok(bookToReturn);
        }
        [ApiExplorerSettings(GroupName = "shared-v1")]
        [Authorize(Roles = "Admin, Librarian")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BookToReturnDto>> DeleteBook(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book is null) return NotFound(new ApiResponse(404));
            var deletedBook = await _bookService.DeleteBookAsync(id);
            var bookToReturn = _mapper.Map<BookToReturnDto>(deletedBook);
            return Ok(bookToReturn);
        }
        [ApiExplorerSettings(GroupName = "public-v1")]
        [HttpGet("TopRatedBooks")]
        public async Task<ActionResult<IReadOnlyList<BookRatingDto>>> GetTopRatedBooks()
        {
            var topRatedBooks = await _bookService.GetTopRatedBooks();
            if (topRatedBooks is null || !topRatedBooks.Any())
                return NotFound(new ApiResponse(404));
            return Ok(topRatedBooks);
        }
    }
}
