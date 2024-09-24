using Microsoft.EntityFrameworkCore;
using System.Reflection;
using SPH.Models;


namespace SPH.Persistence
{
	public class SPHDbContext : DbContext
	{
		public SPHDbContext(DbContextOptions<SPHDbContext> options) : base(options)
		{

		}
        // Indicar los modelos a mapear
        public DbSet<User> Users { get; set; }

        public DbSet<Diary> Diaries { get; set; }

        public DbSet<Organizer> Organizers { get; set; }

		public DbSet<Calendar> Calendars { get; set; }

		public DbSet<Messenger> Messenger { get; set; }

		public DbSet<Message> Message { get; set; }
		public DbSet<Theme> Theme { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			//Indica que la configuracion de los modelos está en un archivo externo: Configuration/ Configurations
			builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
		}
	}
}
