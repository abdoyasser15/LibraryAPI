namespace Library.Core.Entities.Dtos
{
    public class CreateBorrowingDto
    {
        public string UserId { get; set; }
        public int BookId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime BorrowDate { get; set; }
    }
}
