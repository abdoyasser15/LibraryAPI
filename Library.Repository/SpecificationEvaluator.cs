using Library.Core.Entities;
using Library.Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Repository
{
    public static class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> specification)
        {
            var query = inputQuery;
            if(specification.Criteria is not null)
                query = query.Where(specification.Criteria);
            if(specification.OrderBy is not null)
                query = query.OrderBy(specification.OrderBy);
            else if(specification.OrderByDesc is not null)
                query = query.OrderByDescending(specification.OrderByDesc);

            if (specification.IsPagenationEnabled)
                query = query.Skip(specification.Skip).Take(specification.Take);

            query = specification.Include.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
