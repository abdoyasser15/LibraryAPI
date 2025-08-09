using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Specifications.BookSpecs
{
    public class BooksWithFilterationForCountSpecifications : BaseSpecification<Book>
    {
        public BooksWithFilterationForCountSpecifications(BookSpecificationParams specParam)
            :base(b =>
                 (string.IsNullOrEmpty(specParam.Search) || b.Title.ToLower().Contains(specParam.Search))
            && (!specParam.CategoryId.HasValue || b.CategoryId == specParam.CategoryId)
             )
        {
            
        }
    }
}
