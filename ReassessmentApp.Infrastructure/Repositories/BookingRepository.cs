using ReassessmentApp.Domain.Entities;
using ReassessmentApp.Domain.Interfaces;
using ReassessmentApp.Infrastructure.Data;

namespace ReassessmentApp.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(AppDbContext context) : base(context)
        {
        }
    }
}
