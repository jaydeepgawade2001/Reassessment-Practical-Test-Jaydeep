using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReassessmentApp.Application.DTOs;
using ReassessmentApp.Application.Interfaces;
using ReassessmentApp.Domain.Entities;
using ReassessmentApp.Domain.Interfaces;

namespace ReassessmentApp.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository bookingRepository, IRoomRepository roomRepository, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _roomRepository = roomRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var allBookings = await _bookingRepository.GetAllAsync();
            return allBookings.Select(b => new BookingDto
            {
                Id = b.Id,
                RoomId = b.RoomId,
                RoomName = b.Room?.Name ?? "Unknown",
                Title = b.Title,
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                CreatedBy = b.CreatedBy
            });
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByRoomAsync(int roomId)
        {
            var allBookings = await _bookingRepository.GetAllAsync();
            return allBookings
                .Where(b => b.RoomId == roomId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    RoomId = b.RoomId,
                    RoomName = b.Room?.Name ?? "Unknown", // Handle null room
                    Title = b.Title,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    CreatedBy = b.CreatedBy
                });
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return null;

            // Null check for navigation property Room if needed
            var roomName = booking.Room?.Name ?? "Unknown";

            return new BookingDto
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                RoomName = roomName,
                Title = booking.Title,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                CreatedBy = booking.CreatedBy
            };
        }


        public async Task<int> CreateBookingAsync(CreateBookingDto bookingDto)
        {
            _logger.LogInformation("Attempting to create booking for Room {RoomId} from {Start} to {End}", bookingDto.RoomId, bookingDto.StartTime, bookingDto.EndTime);

            // Business Rule 2: Time Validation
            if (bookingDto.StartTime >= bookingDto.EndTime)
            {
                _logger.LogWarning("Validation failed: StartTime after EndTime");
                throw new ArgumentException("StartTime must be before EndTime.");
            }
            if (bookingDto.StartTime < DateTime.UtcNow)
            {
                // Allowing a small buffer or checking against UtcNow is good practice. 
                // Requirement said "Cannot book in the past".
                 throw new ArgumentException("Cannot book in the past.");
            }

            // Business Rule 1: Conflict Detection
            var allBookings = await _bookingRepository.GetAllAsync();
            var isConflict = allBookings.Any(b => 
                b.RoomId == bookingDto.RoomId &&
                ((bookingDto.StartTime >= b.StartTime && bookingDto.StartTime < b.EndTime) || // New start overlaps
                 (bookingDto.EndTime > b.StartTime && bookingDto.EndTime <= b.EndTime) ||     // New end overlaps
                 (bookingDto.StartTime <= b.StartTime && bookingDto.EndTime >= b.EndTime))    // New encompasses old
            );

            if (isConflict)
            {
                _logger.LogWarning("Conflict detected for Room {RoomId} at {Start}", bookingDto.RoomId, bookingDto.StartTime);
                throw new InvalidOperationException("Room is already booked for the selected time slot.");
            }

            var booking = new Booking
            {
                RoomId = bookingDto.RoomId,
                Title = bookingDto.Title,
                StartTime = bookingDto.StartTime,
                EndTime = bookingDto.EndTime,
                CreatedBy = bookingDto.CreatedBy
            };

            await _bookingRepository.AddAsync(booking);
            return booking.Id;
        }

        public async Task CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking != null)
            {
                await _bookingRepository.DeleteAsync(booking);
            }
        }
    }
}
