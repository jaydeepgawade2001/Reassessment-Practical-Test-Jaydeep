using Moq;
using ReassessmentApp.Application.DTOs;
using ReassessmentApp.Application.Services;
using ReassessmentApp.Domain.Entities;
using ReassessmentApp.Domain.Interfaces;
using System.Threading.Tasks;
using Xunit;

namespace ReassessmentApp.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<RoomService>> _mockLogger;
        private readonly RoomService _roomService;

        public RoomServiceTests()
        {
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<RoomService>>();
            _roomService = new RoomService(_mockRoomRepository.Object, _mockBookingRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldReturnId_WhenRoomIsCreated()
        {
            // Arrange
            var roomDto = new CreateRoomDto { Name = "Conference A", Capacity = 10, Location = "Floor 1" };
            var room = new Room { Id = 1, Name = "Conference A", Capacity = 10, Location = "Floor 1" };

            _mockRoomRepository.Setup(r => r.AddAsync(It.IsAny<Room>()))
                .Callback<Room>(r => r.Id = 1) // Simulate DB generating ID
                .Returns(Task.CompletedTask);
            
            // Mock: Unique Name check (returns null aka "not found", so creation proceeds)
            _mockRoomRepository.Setup(r => r.GetByNameAsync(roomDto.Name)).ReturnsAsync((Room?)null);

            // Act
            var result = await _roomService.CreateRoomAsync(roomDto);

            // Assert
            Assert.Equal(1, result);
            _mockRoomRepository.Verify(r => r.AddAsync(It.IsAny<Room>()), Times.Once);
        }

        [Fact]
        public async Task CreateRoomAsync_ShouldThrowException_WhenNameExists()
        {
            // Arrange
            var roomDto = new CreateRoomDto { Name = "Duplicate Room", Capacity = 10, Location = "Floor 1" };
            var existingRoom = new Room { Id = 5, Name = "Duplicate Room", Capacity = 20, Location = "Floor 2" };

            _mockRoomRepository.Setup(r => r.GetByNameAsync(roomDto.Name)).ReturnsAsync(existingRoom);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _roomService.CreateRoomAsync(roomDto));
            _mockRoomRepository.Verify(r => r.AddAsync(It.IsAny<Room>()), Times.Never);
        }
    }
}
