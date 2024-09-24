using SPH.Persistence;
using SPH.Repositories.Interfaces;


namespace SPH.Repositories.Implementations
{
	public class UnitWork : IUnitWork
	{
		private readonly SPHDbContext _db;
		public IUserRepository user { get; private set; }
		public IDiaryRepository diary { get; private set; }
		public IOrganizerRepository organizer { get; private set; }
		public ICalendarRepository calendar { get; private set; }
		public IMessengerRepository messenger { get; private set; }
		public IMessageRepository message { get; private set; }
		public IThemeRepository theme { get; private set; }

        public UnitWork(SPHDbContext db)
		{
			_db = db;
			user = new UserRepository(_db);
			diary = new DiaryRepository(_db);
			organizer = new OrganizerRepository(_db);
			calendar = new CalendarRepository(_db);
			messenger = new MessengerRepository(_db);
			message = new MessageRepository(_db);
			theme = new ThemeRepository(_db);
		}

		public void Dispose()
		{
			_db.Dispose();
		}

        public async Task GuardarAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
