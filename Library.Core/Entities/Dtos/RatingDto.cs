using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Entities.Dtos
{
    public class RatingDto
    {
        public int Id { get; set; }
        public int RaitingValue { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public string UserId { get; set; }
        public int BookId { get; set; }
    }
}
