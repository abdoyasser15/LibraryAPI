using Library.Core;
using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using Library.Core.Specifications.BookSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Book> AddBookAsync(Book book)
        {
            await _unitOfWork.Repository<Book>().AddAsync(book);
            await _unitOfWork.CompleteAsync();
            return book;
        }

        public async Task<Book?> DeleteBookAsync(int id)
        {
           var Book = await _unitOfWork.Repository<Book>().GetByIdAsync(id);
            if(Book is null)
                 return null;
            _unitOfWork.Repository<Book>().Delete(Book);
            await _unitOfWork.CompleteAsync();
            return Book;
        }

        public async Task<IReadOnlyList<Book>> GetAllBooksAsync(BookSpecificationParams specParam)
        {
            var spec = new BookWithCategorySpecification(specParam);
            var Books = await _unitOfWork.Repository<Book>().GetAllWithSpecificationAsync(spec);
            return Books;
        }

        public Task<Book?> GetBookByIdAsync(int BookId)
        {
            var spec = new BookWithCategorySpecification(BookId);
            var Book = _unitOfWork.Repository<Book>().GetWithSpecAsync(spec);
            return Book;
        }

        public Task<int> GetCountAsync(BookSpecificationParams specParam)
        {
            var countSpec = new BooksWithFilterationForCountSpecifications(specParam);
            var count = _unitOfWork.Repository<Book>().GetCountAscync(countSpec);
            return count;
        }

        public async Task UpdateBookAsync(Book book)
        {
            await _unitOfWork.Repository<Book>().GetByIdAsync(book.Id);
            _unitOfWork.Repository<Book>().Update(book);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IReadOnlyList<BookRatingDto>> GetTopRatedBooks(int limit = 10, int minRatings = 1)
        {
            return await _unitOfWork.BookRepository.GetTopRatedBooksAsync(limit, minRatings);
        }
    }
}
