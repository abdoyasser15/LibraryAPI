using System.ComponentModel.DataAnnotations;

namespace LibraTrack.API.DTOs
{
    public class UpdateBookDto
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public int TotalCopies { get; set; }
        [Required]
        public int AvalibleCopies { get; set; }
        [Required]
        public string CoverImageUrl { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
