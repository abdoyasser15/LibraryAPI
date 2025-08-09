using Library.Core;
using Library.Core.Entities;
using Library.Core.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Service
{
    public class CategoryService : GenericService<Category>, ICategoryService 
    {

        public CategoryService(IUnitOfWork unitOfWork)
            :base(unitOfWork)
        {

        }

    }
}
