

namespace SPH.Repositories.Interfaces
{
	public interface IUnitWork : IDisposable
	{
		IUserRepository user { get; }
		IDiaryRepository diary { get; }
		IOrganizerRepository organizer { get; }
		ICalendarRepository calendar { get; }
		IMessengerRepository messenger { get; }
		IMessageRepository message { get; }
		IThemeRepository theme { get; }
		Task GuardarAsync();
	}
}
