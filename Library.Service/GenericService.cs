using Library.Core;
using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class GenericService<T> : IGenericService<T> where T : BaseEntity
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenericService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<T?> AddAsync(T entity)
        {
            await _unitOfWork.Repository<T>().AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }
        public async Task<T?> DeleteAsync(int Id)
        {
            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(Id);
            if (entity is null)
                return null;
            _unitOfWork.Repository<T>().Delete(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            var entities = await _unitOfWork.Repository<T>().GetAllAsync();
            return entities;
        }

        public async Task<T?> GetByIdAsync(int Id)
        {
            var entity = await _unitOfWork.Repository<T>().GetByIdAsync(Id);
            if(entity is null)
                return null;
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            await _unitOfWork.Repository<T>().GetByIdAsync(entity.Id);
            _unitOfWork.Repository<T>().Update(entity);
            await _unitOfWork.CompleteAsync();
        }
    }
}
