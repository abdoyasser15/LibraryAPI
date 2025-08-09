namespace LibraTrack.API.DTOs
{
    public class UpdateFineDto
    {
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
