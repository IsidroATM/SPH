using Microsoft.EntityFrameworkCore;
using SPH.Models;


namespace SPH.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUsuario(string email, string clave);
        Task<User> SaveUsuario(User usuario);
        Task<User> GetUsuarioById(int id);
        Task UpdateUsuario(User usuario);
        Task<User> GetUsuarioByEmail(string email);

    }
}
