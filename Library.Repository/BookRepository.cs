using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.RepositoryContract;
using Library.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repository
{
    public class BookRepository : GenericRepository<Book> ,IBookRepository
    {
        private readonly LibraryContext _context;

        public BookRepository(LibraryContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<BookRatingDto>> GetTopRatedBooksAsync(int limit = 10, int minimumRatings = 10)
        {
            return await _context.Books
                 .Where(b => b.Ratings.Count >= minimumRatings)
                 .Select(b => new BookRatingDto
                 {
                     Id = b.Id,
                     Title = b.Title,
                     AverageRating = b.Ratings.Average(r => r.RaitingValue),
                     TotalRatings = b.Ratings.Count
                 })
                 .OrderByDescending(b => b.AverageRating)
                 .ThenByDescending(b => b.TotalRatings)
                 .Take(limit)
                 .ToListAsync();
        }
    }
}
