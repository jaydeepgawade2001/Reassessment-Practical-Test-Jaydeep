namespace ReassessmentApp.Domain.Entities
{
    public class Room : BaseEntity
    {
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public required string Location { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
