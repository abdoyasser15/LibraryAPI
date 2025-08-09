using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.Specifications.BookSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface IBookService
    {
        Task<IReadOnlyList<Book>> GetAllBooksAsync(BookSpecificationParams specParam);
        Task<Book?> GetBookByIdAsync(int BookId);
        Task<Book?> AddBookAsync(Book book);
        Task UpdateBookAsync(Book book);
        Task<Book?> DeleteBookAsync(int id);
        Task<int> GetCountAsync(BookSpecificationParams specParam);
        Task<IReadOnlyList<BookRatingDto>> GetTopRatedBooks(int limit = 10 , int minRatings = 1);
    }
}
