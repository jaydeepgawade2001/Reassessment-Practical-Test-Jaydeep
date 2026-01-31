using Moq;
using ReassessmentApp.Application.DTOs;
using ReassessmentApp.Application.Services;
using ReassessmentApp.Domain.Entities;
using ReassessmentApp.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ReassessmentApp.Tests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<IRoomRepository> _mockRoomRepository;
        private readonly Mock<Microsoft.Extensions.Logging.ILogger<BookingService>> _mockLogger;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockRoomRepository = new Mock<IRoomRepository>();
            _mockLogger = new Mock<Microsoft.Extensions.Logging.ILogger<BookingService>>();
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockRoomRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowException_WhenStartTimeAfterEndTime()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                RoomId = 1,
                Title = "Test Meeting",
                CreatedBy = "Tester",
                StartTime = DateTime.UtcNow.AddHours(2),
                EndTime = DateTime.UtcNow.AddHours(1) // Invalid
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(dto));
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowException_WhenBookingInPast()
        {
             // Arrange
            var dto = new CreateBookingDto
            {
                RoomId = 1,
                Title = "Test Meeting",
                CreatedBy = "Tester",
                StartTime = DateTime.UtcNow.AddHours(-2), // Past
                EndTime = DateTime.UtcNow.AddHours(-1)
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _bookingService.CreateBookingAsync(dto));
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldThrowException_WhenTimeConflictExists()
        {
            // Arrange
            var existingBookings = new List<Booking>
            {
                new Booking
                {
                    Id = 1,
                    RoomId = 1,
                    Title = "Existing Meeting",
                    CreatedBy = "Existing User",
                    StartTime = DateTime.UtcNow.AddHours(10),
                    EndTime = DateTime.UtcNow.AddHours(12)
                }
            };

            _mockBookingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(existingBookings);

            var newBooking = new CreateBookingDto
            {
                RoomId = 1,
                Title = "New Meeting",
                CreatedBy = "New User",
                StartTime = DateTime.UtcNow.AddHours(11), // Overlaps
                EndTime = DateTime.UtcNow.AddHours(13)
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _bookingService.CreateBookingAsync(newBooking));
            Assert.Equal("Room is already booked for the selected time slot.", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ShouldSuccess_WhenNoConflict()
        {
            // Arrange
            _mockBookingRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Booking>()); // No existing bookings
            _mockBookingRepository.Setup(r => r.AddAsync(It.IsAny<Booking>()))
                .Callback<Booking>(b => b.Id = 100)
                .Returns(Task.CompletedTask);

            var newBooking = new CreateBookingDto
            {
                RoomId = 1,
                Title = "Success Meeting",
                CreatedBy = "Success User",
                StartTime = DateTime.UtcNow.AddDays(1).AddHours(10),
                EndTime = DateTime.UtcNow.AddDays(1).AddHours(12)
            };

            // Act
            var result = await _bookingService.CreateBookingAsync(newBooking);

            // Assert
            Assert.Equal(100, result);
            _mockBookingRepository.Verify(r => r.AddAsync(It.IsAny<Booking>()), Times.Once);
        }
    }
}
