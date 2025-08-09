using Library.Core.Entities;
using Library.Core.Entities.Dtos;
using Library.Service;
using LibraTrack.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.ServiceContract
{
    public interface IFineService : IGenericService<Fine>
    {
        Task<IReadOnlyList<FineDto>> GetAllFinesAsync();
        Task<FineDto?> GetFineByIdAsync(int id);
        Task<FineDto?> GetFineByBorrowingId(int borrowingId);
        Task<IReadOnlyList<FineDto?>> GetFineByUserIdAsync(string userId);
        Task<bool> UpdateFineAsync(int id, UpdateFineDto dto);
        Task<bool> PayFineAsync(int id);
    }
}
