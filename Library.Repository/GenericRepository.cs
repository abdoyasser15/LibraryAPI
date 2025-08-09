using Library.Core.Entities;
using Library.Core.RepositoryContract;
using Library.Core.Specifications;
using Library.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly LibraryContext _dbContext;

        public GenericRepository(LibraryContext libraryContext)
        {
            _dbContext = libraryContext;
        }
        public async Task AddAsync(T entity)
            => await _dbContext.Set<T>().AddAsync(entity);
        public void Delete(T entity)
            =>  _dbContext.Remove(entity);

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>()
                             .Where(predicate)
                             .ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
            => await _dbContext.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAllWithSpecificationAsync(ISpecifications<T> specification)
           => await ApplySpecification(specification).ToListAsync();

        public async Task<T?> GetByIdAsync(object id)
            => await _dbContext.Set<T>().FindAsync(id);

        public Task<int> GetCountAscync(ISpecifications<T> specification)
            => ApplySpecification(specification).CountAsync();

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> specification)
            => await ApplySpecification(specification).FirstOrDefaultAsync();

        public void Update(T entity)
            => _dbContext.Update(entity);
        private IQueryable<T> ApplySpecification(ISpecifications<T> specification)
            => SpecificationEvaluator<T>.GetQuery(_dbContext.Set<T>(), specification);
    }
}
