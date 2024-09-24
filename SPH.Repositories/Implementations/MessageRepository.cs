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
    public class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        private readonly SPHDbContext _db;

        public MessageRepository(SPHDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Message>> GetMessagesByMessengerIdAsync(int messengerId)
        {
            return await _db.Message
                .Where(m => m.MessengerId == messengerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetReceivedMessagesAsync(int receiverId, int messengerId)
        {
            return await _db.Message
                .Where(m => m.ReceiverId == receiverId && m.MessengerId == messengerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetSentMessagesAsync(int senderId, int messengerId)
        {
            return await _db.Message
                .Where(m => m.SenderId == senderId && m.MessengerId == messengerId)
                .ToListAsync();
        }
    }
}
