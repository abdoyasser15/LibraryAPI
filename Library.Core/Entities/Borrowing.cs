using Library.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Entities
{
    public class Borrowing : BaseEntity
    {
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public decimal FineAmount { get; set; }
        public string UserId  { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public Fine Fine { get; set; }
    }
}
