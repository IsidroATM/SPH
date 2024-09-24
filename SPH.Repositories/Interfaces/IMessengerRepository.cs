using SPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Repositories.Interfaces
{
    public interface IMessengerRepository: IRepositoryBase<Messenger>
    {
        Task<IEnumerable<Messenger>> GetUserContactsAsync(int userId);

        //Task<int?> GetMessengerIdAsync(int userId, int userContactId);
        Task<Messenger?> GetMessengerIdAsync(int userId, int userContactId);
    }
}
