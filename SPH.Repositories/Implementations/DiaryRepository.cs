using Microsoft.EntityFrameworkCore;
using SPH.Models;
using SPH.Persistence;
using SPH.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Implementations
{
    public class DiaryRepository: RepositoryBase<Diary>, IDiaryRepository
    {
        private readonly SPHDbContext _db;        
        public DiaryRepository(SPHDbContext db) : base(db)
        {
            _db = db;
        }

        


        public async Task<IEnumerable<Diary>> GetUserNotesAsync(int userId)
        {
            return await ObtenerTodosAsync(note => note.UserId == userId);
        }
    }
}
