using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Core.Entities.Dtos
{
    public class BookRatingDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double AverageRating { get; set; }
        public int TotalRatings { get; set; }
    }
}
