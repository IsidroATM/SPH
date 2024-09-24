using SPH.Models;


namespace SPH.Repositories.Interfaces
{
    public interface ICalendarRepository : IRepositoryBase<Calendar>
    {
        void Actualizar(Calendar calendar);
        Task<IEnumerable<Calendar>> GetUserEventsAsync(int userId);
    }
}
