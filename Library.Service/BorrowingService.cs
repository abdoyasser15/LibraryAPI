using Library.Core;
using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using Library.Repository.Data;
using Library.Repository.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class BorrowingService : GenericService<Borrowing>, IBorrowingService
    {
        private readonly LibraryContext _context;
        private readonly AppIdentityDbContext _identityContext;
        public BorrowingService(IUnitOfWork unitOfWork, LibraryContext context , AppIdentityDbContext identityDbContext)
            :base(unitOfWork)
        {
            _context = context;
            _identityContext = identityDbContext;
        }
        public async Task<IReadOnlyList<BorrowingDto>> GetAllBorrowingsWithUserAsync()
        {
            var borrowings = await (from b in _context.Borrowings
                                   join book in _context.Books on b.BookId equals book.Id
                                   select new
                                   {
                                       b.Id,
                                       b.UserId,
                                       BookId = book.Id,
                                       BookTitle = book.Title,
                                       b.BorrowDate,
                                       b.DueDate,
                                       b.ReturnDate,
                                       b.FineAmount
                                   }).ToListAsync();
            var userIds = borrowings.Select(b => b.UserId).Distinct().ToList();

            var users = await _identityContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id,u.Email })
                .ToListAsync();

            var result = borrowings.Select(b =>
            {
                var user = users.FirstOrDefault(u => u.Id == b.UserId);

                return new BorrowingDto
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    UserId = b.UserId,
                    BorrowedByEmail = user?.Email,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    FineAmount = b.FineAmount
                };
            }).ToList();
            return result;
        }
        public async Task<BorrowingDto?> GetBorrowingByIdAsync(int id)
        {
            var borrowing = await (from b in _context.Borrowings
                             join Book in _context.Books on b.BookId equals Book.Id
                             where b.Id == id
                             select new 
                             {
                                 b.Id,
                                 b.UserId,
                                 BookId = Book.Id,
                                 BookTitle = Book.Title,
                                 b.BorrowDate,
                                 b.DueDate,
                                 b.ReturnDate,
                                 b.FineAmount
                             }
                             ).FirstOrDefaultAsync();
            if (borrowing is null) return null;

            var user = await _identityContext.Users
                .Where(u=>u.Id==borrowing.UserId)
                .Select(u => new { u.Id, u.Email })
                .FirstOrDefaultAsync();
            return new BorrowingDto
            {
                Id = borrowing.Id,
                BookId = borrowing.BookId,
                BookTitle = borrowing.BookTitle,
                UserId = borrowing.UserId,
                BorrowedByEmail = user?.Email,
                BorrowDate = borrowing.BorrowDate,
                DueDate = borrowing.DueDate,
                ReturnDate = borrowing.ReturnDate,
                FineAmount = borrowing.FineAmount
            };
        }
        public async Task<BorrowingDto> AddBorrowingAsync(CreateBorrowingDto Dto)
        {
            var borrowing = new Borrowing
            {
                UserId = Dto.UserId,
                BookId = Dto.BookId,
                BorrowDate = Dto.BorrowDate,
                DueDate = Dto.DueDate,
                ReturnDate = null,
                FineAmount = 0
            };
            _context.Borrowings.Add(borrowing);
            await _context.SaveChangesAsync();

            var book = await _context.Books.FindAsync(Dto.BookId);
            var user = await _identityContext.Users.FindAsync(Dto.UserId);
            return new BorrowingDto
            {
                Id = borrowing.Id,
                UserId = borrowing.UserId,
                BookId = borrowing.BookId,
                BookTitle = book?.Title ?? "Unknown Book",
                BorrowedByEmail = user?.Email ?? "Unknown User",
                BorrowDate = borrowing.BorrowDate,
                DueDate = borrowing.DueDate,
                ReturnDate = borrowing.ReturnDate,
                FineAmount = borrowing.FineAmount
            };
        }
        public async Task<BorrowingDto?> UpdateBorrowingAsync(int id, UpdateBorrowingDto dto)
        {
            var borrowing = await _context.Borrowings.FindAsync(id);
            if(borrowing is null) return null;
            borrowing.ReturnDate = dto.ReturnDate;
            borrowing.DueDate = dto.DueDate;
            borrowing.FineAmount = dto.FineAmount;
            await _context.SaveChangesAsync();

            var book = await _context.Books.FindAsync(borrowing.BookId);
            var user = await _identityContext.Users.FindAsync(borrowing.UserId);

            return new BorrowingDto
            {
                Id = borrowing.Id,
                UserId = borrowing.UserId,
                BookId = borrowing.BookId,
                BookTitle = book?.Title ?? "Unknown Book",
                BorrowedByEmail = user?.Email ?? "Unknown User",
                BorrowDate = borrowing.BorrowDate,
                DueDate = borrowing.DueDate,
                ReturnDate = borrowing.ReturnDate,
                FineAmount = borrowing.FineAmount
            };
        }

        public async Task<IReadOnlyList<BorrowingDto>> GetBorrowingsByUserIdAsync(string userId)
        {
            var borrowings = await (from b in  _context.Borrowings
                                   join book in _context.Books on b.BookId equals book.Id
                                   where b.UserId == userId
                                   select new
                                   {
                                       b.Id,
                                       BookId = book.Id,
                                       BookTitle = book.Title,
                                       b.BorrowDate,
                                       b.DueDate,
                                       b.ReturnDate,
                                       b.FineAmount
                                   }).ToListAsync();
            var user = await _identityContext.Users.Where(u=>u.Id== userId)
                .Select(u => new { u.Id, u.Email })
                .FirstOrDefaultAsync();
            var result = borrowings.Select(b => new BorrowingDto
            {
                Id = b.Id,
                BookId = b.BookId,
                BookTitle = b.BookTitle,
                UserId = userId,
                BorrowedByEmail = user?.Email,
                BorrowDate = b.BorrowDate,
                DueDate = b.DueDate,
                ReturnDate = b.ReturnDate,
                FineAmount = b.FineAmount
            }).ToList();
            return result;
        }

        public async Task<IReadOnlyList<BorrowingDto>> GetBorrowingsOverdueAsync(DateTime currentDate)
        {
            var borrowings = await (from b in _context.Borrowings
                            join book in _context.Books on b.BookId equals book.Id
                            where b.DueDate < currentDate && b.ReturnDate == null
                            select new 
                            {
                                b.Id,
                                BookId = book.Id,
                                BookTitle = book.Title,
                                b.UserId,
                                b.BorrowDate,
                                b.DueDate,
                                b.ReturnDate,
                                b.FineAmount
                            }).ToListAsync();
            var userIds = borrowings.Select(b => b.UserId).Distinct().ToList();
            var users = await _identityContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();
            var result = borrowings.Select(b => 
            {
                var usersDict = users.ToDictionary(u => u.Id, u => u.Email);
                return new BorrowingDto
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    UserId = b.UserId,
                    BorrowedByEmail = usersDict.ContainsKey(b.UserId) ? usersDict[b.UserId] : null,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    FineAmount = b.FineAmount
                };
            }).ToList();
            return result;
        }
        public async Task<IReadOnlyList<BorrowingDto>> GetBorrowingsActiveAsync(DateTime currentDate)
        {
            var borrowings = await(from b in _context.Borrowings
                                   join book in _context.Books on b.BookId equals book.Id
                                   where b.DueDate >= currentDate && b.ReturnDate == null
                                   select new
                                   {
                                       b.Id,
                                       BookId = book.Id,
                                       BookTitle = book.Title,
                                       b.UserId,
                                       b.BorrowDate,
                                       b.DueDate,
                                       b.ReturnDate,
                                       b.FineAmount
                                   }).ToListAsync();
            var userIds = borrowings.Select(b => b.UserId).Distinct().ToList();
            var users = await _identityContext.Users
                .Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();
            var result = borrowings.Select(b =>
            {
                var usersDict = users.ToDictionary(u => u.Id, u => u.Email);
                return new BorrowingDto
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BookTitle = b.BookTitle,
                    UserId = b.UserId,
                    BorrowedByEmail = usersDict.ContainsKey(b.UserId) ? usersDict[b.UserId] : null,
                    BorrowDate = b.BorrowDate,
                    DueDate = b.DueDate,
                    ReturnDate = b.ReturnDate,
                    FineAmount = b.FineAmount
                };
            }).ToList();
            return result;
        }

        public async Task<BorrowingDto?> ReturnBorrowingAsync(int id, DateTime ReturnDate)
        {
            var borrowing = await _context.Borrowings.FindAsync(id);
            if (borrowing is null) return null;
            decimal totalFine = 0m;
            borrowing.ReturnDate = ReturnDate;
            if(ReturnDate>borrowing.DueDate)
            {
                var daysLate = (ReturnDate - borrowing.DueDate).Days;
                decimal finePerDay = 5;
                totalFine = daysLate * finePerDay;
            }
            borrowing.FineAmount = totalFine;
            var fine = new Fine
            {
                    Amount = totalFine,
                    BorrowingId = borrowing.Id,
                    IsPaid = false,
                    PaidAt = null
            };
            await _context.Fines.AddAsync(fine);
            await _context.SaveChangesAsync();
            var book = await _context.Books.FindAsync(borrowing.BookId);
            var user = await _identityContext.Users.FindAsync(borrowing.UserId);
            return new BorrowingDto
            {
                Id = borrowing.Id,
                UserId = borrowing.UserId,
                BookId = borrowing.BookId,
                BookTitle = book?.Title ?? "Unknown Book",
                BorrowedByEmail = user?.Email ?? "Unknown User",
                BorrowDate = borrowing.BorrowDate,
                DueDate = borrowing.DueDate,
                ReturnDate = borrowing.ReturnDate,
                FineAmount = borrowing.FineAmount
            };
        }
    }
}
