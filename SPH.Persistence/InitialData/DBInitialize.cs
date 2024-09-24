
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SPH.Models;
using System;
using System.Linq;

namespace SPH.Persistence.InitialData
{
    public class DBInitialize : IDbIinitialize
    {
        private readonly SPHDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DBInitialize(
            SPHDbContext context,
            RoleManager<IdentityRole> roleManager,
            UserManager<User> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public void Initialize()
        {
            // Aplicar migraciones pendientes si las hay
            try
            {
                if (_context.Database.GetPendingMigrations().Any())
                {
                    _context.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al aplicar migraciones", ex);
            }

            // Crear roles si no existen
            if (!_roleManager.Roles.Any())
            {
                CreateRoles();
            }            

            // Crear usuarios si no existen
            if (!_context.Users.Any())
            {
                CreateUsers();
            }
        }

        private void CreateRoles()
        {
            _roleManager.CreateAsync(new IdentityRole("Administrador")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Instructor")).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole("Estudiante")).GetAwaiter().GetResult();
        }

        private void CreateUsers()
        {
            CreateUser("admin@correo.com", "administrador", "Administrador").GetAwaiter().GetResult();
            CreateUser("instructor@correo.com", "Instructor123*", "Instructor").GetAwaiter().GetResult();
            CreateUser("estudiante@correo.com", "Estudiante123*", "Estudiante").GetAwaiter().GetResult();
        }

        private async Task CreateUser(string email, string password, string role)
        {
            var user = new User
            {
                Nombre = email,
                Correo = email,
                Rol = role,
                Estado = true // Puedes ajustar según necesites
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new Exception($"Error al crear usuario {email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
