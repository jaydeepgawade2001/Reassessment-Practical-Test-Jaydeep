namespace ReassessmentApp.Application.DTOs
{
    public class CreateRoomDto
    {
        public required string Name { get; set; }
        public int Capacity { get; set; }
        public required string Location { get; set; }
    }
}
