using SPH.Models;
using SPH.Persistence;
using SPH.Repositories.Interfaces;

namespace SPH.Repositories.Implementations
{
    public class CalendarRepository : RepositoryBase<Calendar>, ICalendarRepository
    {
        private readonly SPHDbContext _db;

        public CalendarRepository(SPHDbContext db) : base(db)
        {
            _db = db;
        }

        

        public async Task<IEnumerable<Calendar>> GetUserEventsAsync(int userId)
        {
            return await ObtenerTodosAsync(evento => evento.UserId == userId);
        }        
    }
}
