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
    public interface IBorrowingService : IGenericService<Borrowing>
    {
        Task<IReadOnlyList<BorrowingDto>> GetAllBorrowingsWithUserAsync();
        Task<BorrowingDto?> GetBorrowingByIdAsync(int id);
        Task<BorrowingDto> AddBorrowingAsync(CreateBorrowingDto borrowingDto);
        Task<BorrowingDto?> UpdateBorrowingAsync(int id, UpdateBorrowingDto dto);
        Task<IReadOnlyList<BorrowingDto>> GetBorrowingsByUserIdAsync(string userId);
        Task<IReadOnlyList<BorrowingDto>> GetBorrowingsActiveAsync(DateTime currentDate);
        Task<IReadOnlyList<BorrowingDto>> GetBorrowingsOverdueAsync(DateTime currentDate);
        Task<BorrowingDto?> ReturnBorrowingAsync(int id , DateTime ReturnDate);
    }
}
