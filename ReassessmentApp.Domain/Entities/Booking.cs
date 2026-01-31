using System;

namespace ReassessmentApp.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public required string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public required string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
