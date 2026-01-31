using System;

namespace ReassessmentApp.Application.DTOs
{
    public class CreateBookingDto
    {
        public int RoomId { get; set; }
        public required string Title { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public required string CreatedBy { get; set; }
    }
}
