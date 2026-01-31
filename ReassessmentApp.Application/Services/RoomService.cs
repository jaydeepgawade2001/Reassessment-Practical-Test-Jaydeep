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
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository, IBookingRepository bookingRepository, ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
            _bookingRepository = bookingRepository;
            _logger = logger;
            _logger.LogInformation("RoomService initialized");
        }

        public async Task<IEnumerable<RoomDto>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllAsync();
            return rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                Name = r.Name,
                Capacity = r.Capacity,
                Location = r.Location
            });
        }

        public async Task<RoomDto?> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return null;

            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Capacity = room.Capacity,
                Location = room.Location
            };
        }

        public async Task<int> CreateRoomAsync(CreateRoomDto roomDto)
        {
            _logger.LogInformation("Creating new room: {RoomName}", roomDto.Name);

            // Business Rule: Unique Name Check
            var existingRoom = await _roomRepository.GetByNameAsync(roomDto.Name);
            if (existingRoom != null)
            {
                _logger.LogWarning("Room creation failed: Room with name '{RoomName}' already exists.", roomDto.Name);
                throw new ArgumentException($"Room with name '{roomDto.Name}' already exists.");
            }

            var room = new Room
            {
                Name = roomDto.Name,
                Capacity = roomDto.Capacity,
                Location = roomDto.Location,
                Bookings = new List<Booking>()
            };

            await _roomRepository.AddAsync(room);
            return room.Id;
        }

        public async Task DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null) return;

            // Business Rule 4: Delete Rule
            // Check all bookings to see if any belong to this room and are in the future
            var allBookings = await _bookingRepository.GetAllAsync();
            var hasFutureBookings = allBookings.Any(b => b.RoomId == id && b.StartTime > DateTime.UtcNow);

            if (hasFutureBookings)
            {
                _logger.LogWarning("Cannot delete Room {RoomId} because it has future bookings.", id);
                throw new InvalidOperationException("Cannot delete room with future bookings.");
            }

            await _roomRepository.DeleteAsync(room);
        }
    }
}
