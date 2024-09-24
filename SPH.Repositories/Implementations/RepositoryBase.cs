using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SPH.Repositories.Interfaces;
using SPH.Persistence;


namespace SPH.Repositories.Implementations
{
	public class RepositoryBase<T> : IRepositoryBase<T> where T : class
	{
		private readonly SPHDbContext _db;
		internal DbSet<T> dbSet;
		public RepositoryBase(SPHDbContext db)
		{
			_db = db;
			this.dbSet = _db.Set<T>();
		}
		public async Task<T> ObtenerAsync(int id)
		{
			return await dbSet.FindAsync(id); // select * from table where Id = id
		}
		public async Task AgregarAsync(T entity)
		{
			await dbSet.AddAsync(entity); // insert into 
		}

		public void Eliminar(T entity)
		{
			dbSet.Remove(entity);
		}

		public async Task<IEnumerable<T>> ObtenerTodosAsync(
			Expression<Func<T, bool>> filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			string includeProperties = null,
			bool isTracking = true)
		{
			IQueryable<T> query = dbSet;
			if (filter is not null)
				query = query.Where(filter); // select * from where

			if (includeProperties is not null)
			{
				foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProperty); // ejemplo: "Category,Publisher"
				}
			}

			if (orderBy is not null)
				query = orderBy(query);

			if (!isTracking)
				query = query.AsNoTracking();

			return await query.ToListAsync();
		}

        public void Actualizar(T entity)
        {
            dbSet.Update(entity);
        }
    }
}
