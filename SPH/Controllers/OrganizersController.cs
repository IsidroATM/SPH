using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SPH.Models;
using SPH.Persistence;
using SPH.Repositories.Interfaces;
using SPH.Utilities;

namespace SPH.Controllers
{
    [Authorize]
    public class OrganizersController : Controller
    {
        private readonly IUnitWork _unitWork;
        private readonly SPHDbContext _context;

        public OrganizersController(IUnitWork unitWork, SPHDbContext context)
        {
            _unitWork = unitWork;
            _context = context;
        }

        private int GetUserId()
        {
            var claimuser = HttpContext.User;
            int userId = 0;
            if (claimuser.Identity.IsAuthenticated)
            {
                userId = int.Parse(claimuser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            }
            return userId;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            ViewData["UserId"] = userId;

            var tasks = await _unitWork.organizer.GetUserTasksAsync(userId);
            return View(tasks);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var task = new Organizer
            {
                FechaCreacion = DateTime.Now,
                UserId = GetUserId()
            };
            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Organizer task)
        {
            if (ModelState.IsValid)
            {
                task.FechaCreacion = DateTime.Now;

                await _unitWork.organizer.AgregarAsync(task);
                await _unitWork.GuardarAsync();
                TempData[DS.Successful] = "Tarea creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al guardar la tarea, intente de nuevo.";
            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _unitWork.organizer.ObtenerAsync(id.GetValueOrDefault());
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Organizer task)
        {
            if (id != task.TaskId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitWork.organizer.Actualizar(task);
                    await _unitWork.GuardarAsync();
                }
                catch (Exception)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var task = await _unitWork.organizer.ObtenerAsync(id.GetValueOrDefault());
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        
        #region API

        public async Task<IActionResult> Delete(int id)
        {
            var taskDB = await _unitWork.organizer.ObtenerAsync(id);

            if (taskDB is null)
                return Json(new { success = false, message = "Error al eliminar la tarea." });
            var authorBook = await _unitWork.organizer.ObtenerTodosAsync(filter: ab => ab.TaskId == id);
            _unitWork.organizer.Eliminar(taskDB);
            await _unitWork.GuardarAsync();
            return Json(new { success = true, message = "Tarea eliminada correctamente." });
        }
        #endregion
    }
}
