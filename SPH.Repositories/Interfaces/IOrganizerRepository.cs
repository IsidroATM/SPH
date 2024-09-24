using SPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Interfaces
{
    public interface IOrganizerRepository: IRepositoryBase<Organizer>
    {
        void Actualizar(Organizer organizer); // Añadido

        Task<IEnumerable<Organizer>> GetUserTasksAsync(int userId);
    }
}
