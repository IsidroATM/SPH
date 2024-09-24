using SPH.Models;
using SPH.Persistence;
using SPH.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Implementations
{
    public class OrganizerRepository : RepositoryBase<Organizer>, IOrganizerRepository
    {
        private readonly SPHDbContext _db;

        public OrganizerRepository(SPHDbContext db) : base(db)
        {
            _db = db;
        }

               

        public async Task<IEnumerable<Organizer>> GetUserTasksAsync(int userId)
        {
            return await ObtenerTodosAsync(task => task.UserId == userId);
        }
    }
}
