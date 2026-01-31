using System;

namespace ReassessmentApp.Application.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public required string RoomName { get; set; } // Added required
        public required string Title { get; set; }    // Added required
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public required string CreatedBy { get; set; } // Added required
    }
}
