using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Entities
{
    public class Ratings : BaseEntity
    {
        public int RaitingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } // Required for user identification
        public int BookId { get; set; }
        public Book Book { get; set; }
    }
}
