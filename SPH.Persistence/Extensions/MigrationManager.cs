using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Persistence.Extensions
{
    namespace SPH.Repositories
    {
        public class MigrationManager
        {
            private readonly SPHDbContext _context;

            public MigrationManager(SPHDbContext context)
            {
                _context = context;
            }

            public void ApplyMigrations()
            {
                // Implementación para aplicar migraciones aquí
                _context.Database.Migrate();
            }
        }
    }

}
