using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPH.Models;
using SPH.Repositories.Interfaces;
using SPH.Utilities;
using System.Security.Claims;

namespace SPH.Controllers
{
    [Authorize]
    public class MessengersController : Controller
    {
        private readonly IUnitWork _unitWork;

        public MessengersController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        private int GetUserId()
        {
            var claimUser = HttpContext.User;
            int userId = 0;
            if (claimUser.Identity.IsAuthenticated)
            {
                userId = int.Parse(claimUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            ViewData["UserId"] = userId;

            var contacts = await _unitWork.messenger.GetUserContactsAsync(userId);
            return View(contacts);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var messenger = new Messenger
            {
                UserId = GetUserId()
            };
            return View(messenger);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Messenger messenger)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _unitWork.messenger.AgregarAsync(messenger);
                    await _unitWork.GuardarAsync();

                    return Json(new { success = true, message = "Contacto agregado correctamente." });
                }
                catch (Exception)
                {
                    return Json(new { success = false, message = "Error al agregar el contacto, intente de nuevo." });
                }
            }

            return Json(new { success = false, message = "Error en los datos del formulario. Verifique los campos e intente de nuevo." });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var messenger = await _unitWork.messenger.ObtenerAsync(id.GetValueOrDefault());
            if (messenger == null)
            {
                return NotFound();
            }

            return View(messenger);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string content)
        {
            var message = await _unitWork.message.ObtenerAsync(id);
            if (message == null)
            {
                return Json(new { success = false, message = "Mensaje no encontrado." });
            }

            try
            {
                message.Content = content; // Actualizar el contenido del mensaje

                _unitWork.message.Actualizar(message);
                await _unitWork.GuardarAsync();

                return Json(new { success = true, message = "Mensaje editado correctamente." });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Error al editar el mensaje. Inténtelo de nuevo más tarde." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var messenger = await _unitWork.messenger.ObtenerAsync(id.GetValueOrDefault());
            if (messenger == null)
            {
                return NotFound();
            }

            return View(messenger);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var messenger = await _unitWork.messenger.ObtenerAsync(id.GetValueOrDefault());
            if (messenger == null)
                return NotFound();

            return View(messenger);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var messenger = await _unitWork.messenger.ObtenerAsync(id);
            if (messenger == null)
            {
                return Json(new { success = false, message = "Contacto no encontrado." });
            }

            _unitWork.messenger.Eliminar(messenger);
            await _unitWork.GuardarAsync();

            return Json(new { success = true, message = "Contacto eliminado correctamente." });
        }

    }
}