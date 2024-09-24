using SPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Interfaces
{
    public interface IDiaryRepository:IRepositoryBase<Diary>
    {
        void Actualizar(Diary diary); // Añadido
        Task<IEnumerable<Diary>> GetUserNotesAsync(int userId);
    }
}
