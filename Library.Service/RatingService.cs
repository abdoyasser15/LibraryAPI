using Library.Core;
using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Core.ServiceContract;
using Library.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class RatingService : GenericService<Ratings> , IRatingsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly LibraryContext _context;

        public RatingService(IUnitOfWork unitOfWork , LibraryContext context )
            : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<bool> DeleteRatingAsync(int id)
        {
           var existingRating = await _unitOfWork.Repository<Ratings>().GetByIdAsync(id);
            if (existingRating is null)
                return false;
             _unitOfWork.Repository<Ratings>().Delete(existingRating);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }

        public async Task<IReadOnlyList<RatingDto>> GetAllRatingsAsync()
        {
            var ratings = await _unitOfWork.Repository<Ratings>().GetAllAsync();
            var ratingDtos = ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                RaitingValue = r.RaitingValue,
                Comment = r.Comment,
                UserId = r.UserId,
                BookId = r.BookId,
                CreatedAt = r.CreatedAt
            }).ToList();
            return ratingDtos;
        }

        public async Task<IReadOnlyList<RatingDto?>> GetRatingByBookId(int bookId)
        {
            var ratings = await _context.Ratings
                                .Where(r => r.BookId == bookId)
                                .ToListAsync();

            var result = ratings.Select(r => new RatingDto
            {
                Id = r.Id,
                RaitingValue = r.RaitingValue,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                UserId = r.UserId,
                BookId = r.BookId
            }).ToList();
            return result;
        }

        public async Task<RatingDto?> GetRatingByIdAsync(int id)
        {
            var rating = await _unitOfWork.Repository<Ratings>().GetByIdAsync(id);
            var ratingDto = rating != null ? new RatingDto
            {
                Id = rating.Id,
                RaitingValue = rating.RaitingValue,
                Comment = rating.Comment,
                UserId = rating.UserId,
                BookId = rating.BookId,
                CreatedAt = rating.CreatedAt
            } : null;
            return ratingDto;
        }

        public async Task<bool> UpdateRatingAsync(int id, RatingDto rating)
        {
            var existingRating = await _unitOfWork.Repository<Ratings>().GetByIdAsync(id);

            if (existingRating is null)
                return false;
            existingRating.RaitingValue = rating.RaitingValue;
            existingRating.Comment = rating.Comment;
            existingRating.UserId = rating.UserId;
            existingRating.BookId = rating.BookId;
            existingRating.CreatedAt = rating.CreatedAt;

            _unitOfWork.Repository<Ratings>().Update(existingRating);
            var result = await _unitOfWork.CompleteAsync();
            return result > 0;
        }
    }
}
