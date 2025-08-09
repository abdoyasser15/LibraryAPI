using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Specifications
{
    public class BaseSpecification<T> : ISpecifications<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set;}
        public List<Expression<Func<T, object>>> Include { get ; set ; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get ; set; }
        public Expression<Func<T, object>> OrderByDesc { get ; set; }
        public int Skip { get ; set; }
        public int Take { get; set; }
        public bool IsPagenationEnabled { get; set; }
        public BaseSpecification()
        {
            
        }
        public BaseSpecification(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }
        public void AddOrderBy(Expression<Func<T, object>> OrderByExpresion)
        {
            OrderBy = OrderByExpresion;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> OrderByExpresion)
        {
            OrderByDesc = OrderByExpresion;
        }
        public void ApplyPagenation(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagenationEnabled = true;
        }
    }
}
