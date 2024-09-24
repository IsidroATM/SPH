using Microsoft.EntityFrameworkCore;
using SPH.Models;
using SPH.Persistence;
using SPH.Repositories.Interfaces;
using System.Diagnostics;

namespace SPH.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        
        private readonly SPHDbContext _db;

        public UserRepository( SPHDbContext db )
        {
            _db = db;
        }
        public async Task<User> GetUsuario(string email, string clave)
        {
            User user = await _db.Users.Where(u=>u.Correo == email && u.Contraseña == clave).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> SaveUsuario(User usuario)
        {
            _db.Users.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }
        public async Task<User> GetUsuarioById(int id)
        {
            User user = await _db.Users.FindAsync(id);
            return user;
        }
        public async Task UpdateUsuario(User usuario)
        {
            _db.Entry(usuario).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }
        public async Task<User> GetUsuarioByEmail(string email)
        {
            // Busca el usuario por correo electrónico
            User user = await _db.Users.FirstOrDefaultAsync(u => u.Correo == email);
            return user;
        }
    }
}

