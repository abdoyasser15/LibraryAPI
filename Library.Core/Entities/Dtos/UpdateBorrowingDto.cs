using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Entities.Dtos
{
    public class UpdateBorrowingDto
    {
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal FineAmount { get; set; } = 0;
    }
}
