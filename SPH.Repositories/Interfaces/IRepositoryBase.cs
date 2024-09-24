using SPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Interfaces
{
	public interface IRepositoryBase<T> where T : class
	{		
		// Listar registros de acuerdo al Id
		Task<T> ObtenerAsync(int id);

		// Listar todos los registros
		Task<IEnumerable<T>> ObtenerTodosAsync(
			Expression<Func<T, bool>> filter = null, // Filtros
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, // Ordenamiento
			string includeProperties = null, // Inner Join
			bool isTracking = true // Seguimiento (Tracking)
			);

		// Agregar un registro
		Task AgregarAsync(T entity);

		// Eliminar un registro
		void Eliminar(T entity);

        // Actualizar un registro
        void Actualizar(T entity);
    }
}

