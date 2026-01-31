using System.Collections.Generic;
using System.Threading.Tasks;
using ReassessmentApp.Application.DTOs;

namespace ReassessmentApp.Application.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomDto>> GetAllRoomsAsync();
        Task<RoomDto?> GetRoomByIdAsync(int id);
        Task<int> CreateRoomAsync(CreateRoomDto roomDto);
        Task DeleteRoomAsync(int id);
    }
}
