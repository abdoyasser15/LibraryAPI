using Library.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Specifications.BookSpecs
{
    public class BookWithCategorySpecification : BaseSpecification<Book>
    {
        public BookWithCategorySpecification(BookSpecificationParams specParam)
            :base(b=>
                 (string.IsNullOrEmpty(specParam.Search)||b.Title.ToLower().Contains(specParam.Search))
            && (!specParam.CategoryId.HasValue||b.CategoryId==specParam.CategoryId)
             )
        {
            AddIncludes();
            if (!string.IsNullOrEmpty(specParam.Sort))
            {
                switch (specParam.Sort.ToLower())
                {
                    case "titleasc":
                        AddOrderBy(B => B.Title);
                        break;
                    case "titledesc":
                        AddOrderByDesc(B => B.Title);
                        break;
                    case "authorasc":
                        AddOrderBy(B => B.Author);
                        break;
                    case "authordesc":
                        AddOrderByDesc(B => B.Author);
                        break;
                }
            }
            ApplyPagenation((specParam.PageIndex - 1) * specParam.PageSize, specParam.PageSize);
        }
        public BookWithCategorySpecification(int id)
            : base(b => b.Id == id)
        {
            AddIncludes();
        }
        private void AddIncludes()
        {
            Include.Add(b => b.Category);
        }
    }
}
