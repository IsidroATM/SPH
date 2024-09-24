using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPH.Repositories.Interfaces;
using System.Security.Claims;
using SPH.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SPH.Repositories.Implementations;
using SPH.Utilities;
using Microsoft.EntityFrameworkCore;

namespace SPH.Controllers
{
    [Authorize]
    public class MessagesController : Controller
    {
        private readonly IUnitWork _unitWork;

        public MessagesController(IUnitWork unitWork)
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

        private async Task<int> GetReceiverId(int messengerId)
        {
            var messenger = await _unitWork.messenger.ObtenerAsync(messengerId);
            if (messenger != null)
            {
                var userId = GetUserId(); // Obtener el ID del usuario actualmente autenticado
                var creatorId = messenger.UserId; // Obtener el ID del usuario creador del canal

                // Determinar quién es el receptor en función del usuario actual
                if (userId == creatorId)
                {
                    // El usuario actual es el creador del canal, enviar al usuario objetivo
                    return messenger.UserContactId;
                }
                else
                {
                    // El usuario actual es el usuario objetivo, enviar al creador del canal
                    return messenger.UserId;
                }
            }
            return 0; // O devuelve un valor predeterminado según tu lógica
        }

        [HttpGet]
        public async Task<IActionResult> Index(int messengerId)
        {
            var userId = GetUserId();

            var sentMessages = await _unitWork.message.GetSentMessagesAsync(userId, messengerId);
            var receivedMessages = await _unitWork.message.GetReceivedMessagesAsync(userId, messengerId);

            ViewData["MessengerId"] = messengerId;
            ViewData["UserId"] = userId;
            ViewData["UserContactId"] = 0; // Asegúrate de establecer esto según tu lógica para el contacto del usuario

            var model = new Tuple<IEnumerable<Message>, IEnumerable<Message>>(sentMessages, receivedMessages);

            return View(model);
        }       

        [HttpGet]
        public async Task<IActionResult> Create(int messengerId)
        {
            var message = new Message
            {
                MessengerId = messengerId,
                SenderId = GetUserId()
            };

            // Obtener el UserContactId y asignarlo a ViewData
            var receiver = await GetUserContactAsync(messengerId);
            if (receiver != null)
            {
                ViewData["UserContactId"] = receiver.Id;
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int messengerId, IFormFile multimediaFile, string content)
        {
            try
            {
                var userId = GetUserId();
                var receiverId = await GetReceiverId(messengerId);

                var message = new Message
                {
                    Content = content,
                    Timestamp = DateTime.Now,
                    SenderId = userId,
                    ReceiverId = receiverId,
                    MessengerId = messengerId
                };

                // Procesar el archivo multimedia si se adjuntó
                if (multimediaFile != null && multimediaFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await multimediaFile.CopyToAsync(memoryStream);
                        message.MultimediaContent = memoryStream.ToArray();
                        message.MultimediaContentType = multimediaFile.ContentType;
                    }
                }

                await _unitWork.message.AgregarAsync(message);
                await _unitWork.GuardarAsync();

                TempData["Successful"] = "Mensaje enviado correctamente.";

                // Redirigir de regreso a la vista Index o Detail para actualizar los mensajes
                return RedirectToAction("Index", new { messengerId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al enviar el mensaje. Por favor, inténtelo de nuevo.";
                // Registrar la excepción o manejarla adecuadamente según tus necesidades
                return RedirectToAction("Index", new { messengerId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var message = await _unitWork.message.ObtenerAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var message = await _unitWork.message.ObtenerAsync(id.GetValueOrDefault());
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Message message, IFormFile? MultimediaFile)
        {
            if (id != message.MessageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingMessage = await _unitWork.message.ObtenerAsync(message.MessageId);

                    // Si se proporciona un nuevo archivo multimedia, actualiza el contenido y el tipo
                    if (MultimediaFile != null && MultimediaFile.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await MultimediaFile.CopyToAsync(ms);
                            message.MultimediaContent = ms.ToArray();
                            message.MultimediaContentType = MultimediaFile.ContentType;
                        }
                    }
                    else
                    {
                        // Mantener el contenido multimedia existente si no se proporciona uno nuevo
                        message.MultimediaContent = existingMessage.MultimediaContent;
                        message.MultimediaContentType = existingMessage.MultimediaContentType;
                    }

                    // Actualizar la hora al momento actual
                    message.Timestamp = DateTime.Now;

                    try
                    {
                        _unitWork.message.Actualizar(message);
                        await _unitWork.GuardarAsync();
                        TempData["Successful"] = "Mensaje editado correctamente.";

                        return RedirectToAction("Index", new { messengerId = message.MessengerId });
                    }
                    catch (DbUpdateException ex)
                    {
                        // Log the exception
                        TempData["Error"] = "Error al guardar los cambios en la base de datos. Inténtelo de nuevo.";
                        return View(message); // Mostrar vista de edición con mensaje de error
                    }
                }
                catch (Exception)
                {
                    TempData["Error"] = "Error al editar el mensaje. Inténtelo de nuevo.";
                    return View(message); // Mostrar vista de edición con mensaje de error
                }
            }

            return View(message); // Mostrar vista de edición con errores de validación
        }









        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var message = await _unitWork.message.ObtenerAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var message = await _unitWork.message.ObtenerAsync(id);
                if (message == null)
                {
                    return Json(new { success = false, error = "El mensaje no fue encontrado." });
                }

                _unitWork.message.Eliminar(message);
                await _unitWork.GuardarAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = "Error al eliminar el mensaje: " + ex.Message });
            }            
        }
         





        private async Task<User?> GetUserContactAsync(int messengerId)
        {
            var messenger = await _unitWork.messenger.ObtenerAsync(messengerId);
            return messenger?.UserContact;
        }
    }
}
