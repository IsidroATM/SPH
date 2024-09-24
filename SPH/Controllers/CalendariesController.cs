using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPH.Models;
using SPH.Repositories.Interfaces;
using SPH.Utilities;
using System.Security.Claims;

namespace SPH.Controllers
{
    [Authorize]
    public class CalendariesController : Controller
    {
        private readonly IUnitWork _unitWork;

        public CalendariesController(IUnitWork unitWork)
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

            var events = await _unitWork.calendar.GetUserEventsAsync(userId);
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var calendar = new Calendar
            {
                FechaIniEvento = DateTime.Now,
                FechaFinEvento = DateTime.Now,
                UserId = GetUserId()
            };
            return View(calendar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Calendar calendar)
        {
            if (ModelState.IsValid)
            {
                await _unitWork.calendar.AgregarAsync(calendar);
                await _unitWork.GuardarAsync();
                TempData[DS.Successful] = "Evento creado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar el evento, intente de nuevo.";
            return View(calendar);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var calendar = await _unitWork.calendar.ObtenerAsync(id.GetValueOrDefault());
            if (calendar == null)
            {
                return NotFound();
            }

            return View(calendar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Calendar calendar)
        {
            if (id != calendar.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitWork.calendar.Actualizar(calendar);
                    await _unitWork.GuardarAsync();
                    TempData["Successful"] = "Evento actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["Error"] = "Error al actualizar el evento, intente de nuevo.";
                    return View(calendar);
                }
            }
            return View(calendar);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calendar = await _unitWork.calendar.ObtenerAsync(id.GetValueOrDefault());
            if (calendar == null)
            {
                return NotFound();
            }

            return View(calendar);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var calendar = await _unitWork.calendar.ObtenerAsync(id.GetValueOrDefault());
            if (calendar == null)
                return NotFound();

            return View(calendar);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var calendar = await _unitWork.calendar.ObtenerAsync(id);
            if (calendar == null)
            {
                return Json(new { success = false, message = "Evento no encontrado." });
            }

            _unitWork.calendar.Eliminar(calendar);
            await _unitWork.GuardarAsync();

            return Json(new { success = true, message = "Evento eliminado correctamente." });
        }
    }
}
