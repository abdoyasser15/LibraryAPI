namespace LibraTrack.API.DTOs
{
    public class BookToReturnDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string CoverImageUrl { get; set; }
        public string Description { get; set; }
        public int TotalCopies { get; set; }
        public int AvalibleCopies { get; set; }
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}
