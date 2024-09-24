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
    public class MessengerRepository : RepositoryBase<Messenger>, IMessengerRepository
    {
        private readonly SPHDbContext _db;

        public MessengerRepository(SPHDbContext db) : base(db)
        {
            _db = db;
        }

        //public async Task<IEnumerable<Messenger>> GetUserContactsAsync(int userId)
        //{
        //    return await ObtenerTodosAsync(contact => contact.UserId == userId);
        //}
        public async Task<IEnumerable<Messenger>> GetUserContactsAsync(int userId)
        {
            return await _db.Messenger
                .Where(m => m.UserId == userId || m.UserContactId == userId)
                .ToListAsync();
        }

        public async Task<Messenger?> GetMessengerIdAsync(int userId, int userContactId)
        {
            return await _db.Messenger
                .FirstOrDefaultAsync(m =>
                    (m.UserId == userId && m.UserContactId == userContactId) ||
                    (m.UserId == userContactId && m.UserContactId == userId));
        }

        public async Task<string?> GetUserNameAsync(int userId)
        {
            var user = await _db.Users.FindAsync(userId);
            return user?.Nombre;
        }
    }
}
