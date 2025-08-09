using Library.Core.Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.RepositoryContract
{
    public interface IBookRepository
    {
        Task<List<BookRatingDto>> GetTopRatedBooksAsync(int limit = 10, int minimumRatings = 10);
    }
}
