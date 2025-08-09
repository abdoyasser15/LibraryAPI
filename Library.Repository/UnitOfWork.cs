using Library.Core;
using Library.Core.Entities;
using Library.Core.RepositoryContract;
using Library.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryContext _dbContext;
        Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private IBookRepository _bookRepository;
        public IBookRepository BookRepository
        {
            get
            {
                if (_bookRepository == null)
                {
                    _bookRepository = new BookRepository(_dbContext);
                }
                return _bookRepository;
            }
        }
        public UnitOfWork(LibraryContext libraryContext)
        {
            _dbContext = libraryContext;
        }
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            var key = typeof(TEntity);
            if (!_repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<TEntity>(_dbContext);
                _repositories.Add(key, repository);
            }
            return (IGenericRepository<TEntity>)_repositories[key];
        }
        public Task<int> CompleteAsync()
            => _dbContext.SaveChangesAsync();

        public ValueTask DisposeAsync()
            => _dbContext.DisposeAsync();

    }
}
