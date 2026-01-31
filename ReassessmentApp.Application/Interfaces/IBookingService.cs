using System.Collections.Generic;
using System.Threading.Tasks;
using ReassessmentApp.Application.DTOs;

namespace ReassessmentApp.Application.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync(); // New method
        Task<IEnumerable<BookingDto>> GetBookingsByRoomAsync(int roomId);
        Task<BookingDto?> GetBookingByIdAsync(int id);
        Task<int> CreateBookingAsync(CreateBookingDto bookingDto);
        Task CancelBookingAsync(int id);
    }
}
