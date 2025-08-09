using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public interface IGenericService<T> where T : BaseEntity
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int Id);
        Task UpdateAsync(T entity);
        Task<T?> DeleteAsync(int Id);
        Task<T?> AddAsync(T entity);
    }
}
