using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SPH.Models;

namespace SPH.Persistence.InitialData
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new SPHDbContext(serviceProvider.GetRequiredService<DbContextOptions<SPHDbContext>>()))
            {
                if (context.Theme.Any())
                    return;

                // Insertar temas iniciales
                context.Theme.AddRange(
                    new Theme { Name = "Default", Color = "#ffffff", FontType = "Arial", FontSize = 12 },
                    new Theme { Name = "Dark Mode", Color = "#000000", FontType = "Verdana", FontSize = 14 }
                    // Agregar otros temas según sea necesario
                );

                context.SaveChanges();
            }
        }
    }
}
