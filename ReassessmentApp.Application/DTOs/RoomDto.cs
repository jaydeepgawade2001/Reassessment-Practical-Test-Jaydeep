namespace ReassessmentApp.Application.DTOs
{
    public class RoomDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public required string Location { get; set; }
    }
}
