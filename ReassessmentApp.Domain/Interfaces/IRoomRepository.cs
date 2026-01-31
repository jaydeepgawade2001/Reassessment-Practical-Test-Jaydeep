using ReassessmentApp.Domain.Entities;

namespace ReassessmentApp.Domain.Interfaces
{
    public interface IRoomRepository : IGenericRepository<Room>
    {
        // Add specific methods if needed
        Task<Room?> GetByNameAsync(string name);
    }
}
