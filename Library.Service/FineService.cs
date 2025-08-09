using Library.Core;
using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using Library.Repository.Data;
using Library.Repository.Identity;
using LibraTrack.API.DTOs;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class FineService : GenericService<Fine>, IFineService
    {
        private readonly LibraryContext _context;
        private readonly AppIdentityDbContext _identityContext;
        private readonly INotificationService _notificationService;

        public FineService(IUnitOfWork unitOfWork , LibraryContext context, 
            AppIdentityDbContext identityContext , INotificationService notificationService)
            :base(unitOfWork)
        {
            _context = context;
            _identityContext = identityContext;
            _notificationService = notificationService;
        }
        public async Task<IReadOnlyList<FineDto>> GetAllFinesAsync()
        {
            var fines = await (from f in _context.Fines
                               join b in _context.Borrowings on f.BorrowingId equals b.Id
                               join book in _context.Books on b.BookId equals book.Id
                               select new
                               {
                                   f.Id,
                                   f.Amount,
                                   f.IsPaid,
                                   f.PaidAt,
                                   BorrowinId = b.Id,
                                   b.BorrowDate,
                                   b.DueDate,
                                   b.UserId,
                                   book.Title
                               }).ToListAsync();
            var userIds = fines.Select(U => U.UserId).Distinct().ToList();
            var users = await _identityContext.Users.Where(u => userIds.Contains(u.Id))
                .Select(u => new { u.Id, u.Email })
                .ToListAsync();
            var result = fines.Select(f =>
            {
                var user = users.FirstOrDefault(u => u.Id == f.UserId);

                return new FineDto
                {
                    Id = f.Id,
                    Amount = f.Amount,
                    IsPaid = f.IsPaid,
                    PaidAt = f.PaidAt,
                    BorrowingId = f.BorrowinId,
                    BorrowDate = f.BorrowDate,
                    DueDate = f.DueDate,
                    BookTitle = f.Title,
                    UserId = f.UserId,
                    UserEmail = user?.Email
                };
            }).ToList();
            return result;
        }
        public async Task<FineDto?> GetFineByIdAsync(int id)
        {
            var fine = await (from f in _context.Fines
                              join b in _context.Borrowings on f.BorrowingId equals b.Id
                              join book in _context.Books on b.BookId equals book.Id
                              where f.Id == id
                              select new
                              {
                                  f.Id,
                                  f.Amount,
                                  f.IsPaid,
                                  f.PaidAt,
                                  BorrowinId = b.Id,
                                  b.BorrowDate,
                                  b.DueDate,
                                  b.UserId,
                                  book.Title
                              }).FirstOrDefaultAsync();
            if (fine is null) return null;
            var userId = fine.UserId;
            var user = await _identityContext.Users.Where(U=>U.Id==userId)
                .FirstOrDefaultAsync();
            return new FineDto
            {
                Id = fine.Id,
                Amount = fine.Amount,
                IsPaid = fine.IsPaid,
                PaidAt = fine.PaidAt,
                BorrowingId = fine.BorrowinId,
                BorrowDate = fine.BorrowDate,
                DueDate = fine.DueDate,
                BookTitle = fine.Title,
                UserId = fine.UserId,
                UserEmail = user?.Email
            };
        }
        public async Task<FineDto?> GetFineByBorrowingId(int borrowingId)
        {
            var fine = await (from f in _context.Fines
                              join b in _context.Borrowings on f.BorrowingId equals b.Id
                              join book in _context.Books on b.BookId equals book.Id
                              where f.BorrowingId == borrowingId
                              select new
                              {
                                  f.Id,
                                  f.Amount,
                                  f.IsPaid,
                                  f.PaidAt,
                                  BorrowingId = b.Id,
                                  b.BorrowDate,
                                  b.DueDate,
                                  b.UserId,
                                  book.Title
                              }).FirstOrDefaultAsync();
            if (fine is null) return null;
            var user = await _identityContext.Users.FindAsync(fine.UserId);
            return new FineDto
            {
                Id = fine.Id,
                Amount = fine.Amount,
                IsPaid = fine.IsPaid,
                PaidAt = fine.PaidAt,
                BorrowingId = fine.BorrowingId,
                BorrowDate = fine.BorrowDate,
                DueDate = fine.DueDate,
                BookTitle = fine.Title,
                UserId = fine.UserId,
                UserEmail = user?.Email
            };
        }

        public async Task<IReadOnlyList<FineDto?>> GetFineByUserIdAsync(string userId)
        {
            var fines = await(from f in _context.Fines
                              join b in _context.Borrowings on f.BorrowingId equals b.Id
                              join book in _context.Books on b.BookId equals book.Id
                              select new
                              {
                                  f.Id,
                                  f.Amount,
                                  f.IsPaid,
                                  f.PaidAt,
                                  BorrowingId = b.Id,
                                  b.BorrowDate,
                                  b.DueDate,
                                  b.UserId,
                                  book.Title
                              }).ToListAsync();
            var user = await _identityContext.Users.FirstOrDefaultAsync(U => U.Id == userId);
            var result = fines.Select(f => new FineDto
            {
                Id = f.Id,
                Amount = f.Amount,
                IsPaid = f.IsPaid,
                PaidAt = f.PaidAt,
                BorrowingId = f.Id,
                BorrowDate = f.BorrowDate,
                DueDate = f.DueDate,
                BookTitle = f.Title,
                UserId = f.UserId,
                UserEmail = user?.Email
            }
            ).ToList();
            return result;
        }

        public async Task<bool> UpdateFineAsync(int id, UpdateFineDto dto)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine is null)
                return false;
            fine.IsPaid = dto.IsPaid;
            fine.PaidAt = dto.IsPaid?dto.PaidAt??DateTime.UtcNow:null;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PayFineAsync(int id)
        {
            var fine = await _context.Fines
                .Include(f => f.Borrowing)
                .FirstOrDefaultAsync(f => f.Id == id);
            if (fine is null || fine.IsPaid)
                return false;
            fine.IsPaid = true;
            fine.PaidAt = DateTime.UtcNow;
            _context.Fines.Update(fine);
            await _context.SaveChangesAsync();
            var userId = fine.Borrowing?.UserId;
            if (!string.IsNullOrEmpty(userId))
            {
                await _notificationService.SendNotificationAsync(
                 userId,
                 "Fine Payment Notification",
                 $"Your fine has been paid {fine.Amount} successfully."
                );
            }
            return true;
        }
    }
}
