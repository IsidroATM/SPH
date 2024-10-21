using Microsoft.AspNetCore.Mvc;
using SPH.Models;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using SPH.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace SPH.Controllers
{
   
    public class UsersController : Controller
    {        
        private readonly IUnitWork _unitWork;

        
        public UsersController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public IActionResult Index() 
        {            
            return View(); 
        }

        public IActionResult Register() { return View(); }

        
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
          
            var existingUser = await _unitWork.user.GetUsuarioByEmail(user.Correo);

            if (existingUser != null)
            {
              
                ViewData["Mensaje"] = "El correo ya está registrado. Por favor, intenta con otro correo.";
                return View();
            }

            user.Contraseña = Utilities.Serv_Encrip.EncriptarClave(user.Contraseña);
            user.Rol = "Estudiante"; 
            user.Estado = true;
            user.Biografia = "";
            user.ImagenPerfil = "";
            user.NumeroContacto = "";
            user.Ubicacion = "";
            user.Organizacion = "";

            try
            {
               
                User user_c = await _unitWork.user.SaveUsuario(user);

                if (user_c.Id > 0)
                {
                    await _unitWork.GuardarAsync(); 

                    return RedirectToAction("Login", "Users");
                }

                ViewData["Mensaje"] = "No se pudo crear el Usuario";
            }
            catch (Exception ex)
            {
                ViewData["Mensaje"] = "Ocurrió un error al intentar crear el usuario: " + ex.Message;
            }
            return View();
        }



        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string clave)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
            {
                ViewData["Mensaje"] = "Correo y contraseña son requeridos";
                return View();
            }

            User user_enc = await _unitWork.user.GetUsuario(email, Utilities.Serv_Encrip.EncriptarClave(clave));

            if (user_enc == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            // Objeto que almacene la información de los claims
            List<Claim> claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, user_enc.Nombre), // Claim de nombre del usuario
                new Claim(ClaimTypes.NameIdentifier, user_enc.Id.ToString()), // Claim de ID del usuario
                new Claim(ClaimTypes.Role, user_enc.Rol) // Claim del rol del usuario
            };

            // Crear una identidad de claims
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            // Iniciar sesión con las claims proporcionadas
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Details()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _unitWork.user.GetUsuarioById(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Edit()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            User user = await _unitWork.user.GetUsuarioById(userId);

            if (user == null)
            {
                return NotFound();
            }
            // Ocultar Rol y Estado para que no sean modificables desde la vista de edición
            ModelState.Remove("Rol");
            ModelState.Remove("Estado");

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Encriptar la contraseña si se ha proporcionado
            if (!string.IsNullOrEmpty(user.Contraseña))
            {
                user.Contraseña = Utilities.Serv_Encrip.EncriptarClave(user.Contraseña);
            }

            // Asignar "" a los campos nulos antes de actualizar
            if (string.IsNullOrEmpty(user.Biografia))
            {
                user.Biografia = "";
            }
            if (string.IsNullOrEmpty(user.ImagenPerfil))
            {
                user.ImagenPerfil = "";
            }
            if (string.IsNullOrEmpty(user.NumeroContacto))
            {
                user.NumeroContacto = "";
            }
            if (string.IsNullOrEmpty(user.Ubicacion))
            {
                user.Ubicacion = "";
            }
            if (string.IsNullOrEmpty(user.Organizacion))
            {
                user.Organizacion = "";
            }

            await _unitWork.user.UpdateUsuario(user);

            return RedirectToAction("Details");
        }
        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }
        
    }
}

