using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Specifications.BookSpecs
{
    public class BookSpecificationParams
    {
        private const int maxPageSize = 10;
        public int? CategoryId { get; set; }
        public string? Sort { get; set; }

        private string? search;

        public string? Search
        {
            get { return search; }
            set { search = value?.ToLower(); }
        }
        private int pageSize;

        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > maxPageSize ? maxPageSize: value; }
        }
        public int PageIndex { get; set; } = 1;
    }
}
