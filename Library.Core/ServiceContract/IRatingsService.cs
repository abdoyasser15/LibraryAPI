using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface IRatingsService : IGenericService<Ratings>
    {
        Task<IReadOnlyList<RatingDto>> GetAllRatingsAsync();
        Task<RatingDto?> GetRatingByIdAsync(int id);
        Task<IReadOnlyList<RatingDto?>> GetRatingByBookId(int bookId);
        Task<bool> UpdateRatingAsync(int id, RatingDto rating);
        Task<bool> DeleteRatingAsync(int id);
    }
}
