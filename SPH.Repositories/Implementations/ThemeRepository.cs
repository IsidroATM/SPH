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
    public class ThemeRepository : RepositoryBase<Theme>, IThemeRepository
    {
        private readonly SPHDbContext _db;
        public ThemeRepository(SPHDbContext db) : base(db)
        {
            _db = db;
        }

       
    }
}
