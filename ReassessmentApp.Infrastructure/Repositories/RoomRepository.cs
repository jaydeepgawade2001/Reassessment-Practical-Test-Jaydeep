using Microsoft.EntityFrameworkCore;
using ReassessmentApp.Domain.Entities;
using ReassessmentApp.Domain.Interfaces;
using ReassessmentApp.Infrastructure.Data;

namespace ReassessmentApp.Infrastructure.Repositories
{
    public class RoomRepository : GenericRepository<Room>, IRoomRepository
    {
        public RoomRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Room?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
