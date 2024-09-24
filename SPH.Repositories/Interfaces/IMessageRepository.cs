using SPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Interfaces
{
    public interface IMessageRepository : IRepositoryBase<Message>
    {
        Task<IEnumerable<Message>> GetMessagesByMessengerIdAsync(int messengerId);
        Task<IEnumerable<Message>> GetSentMessagesAsync(int senderId, int messengerId);
        Task<IEnumerable<Message>> GetReceivedMessagesAsync(int receiverId, int messengerId);
    }
}
