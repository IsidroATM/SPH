using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPH.Models;
using SPH.Repositories.Interfaces;

namespace SPH.Controllers
{
    [Authorize]
    public class ThemesController : Controller
    {
        private readonly IUnitWork _unitWork;

        public ThemesController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public async Task<IActionResult> Index()
        {
            var themes = await _unitWork.theme.ObtenerTodosAsync();
            return View(themes);
        }

        public async Task<IActionResult> Details(int id)
        {
            var theme = await _unitWork.theme.ObtenerAsync(id);
            if (theme == null)
            {
                return NotFound();
            }
            return View(theme);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Theme theme)
        {
            if (ModelState.IsValid)
            {
                await _unitWork.theme.AgregarAsync(theme);
                await _unitWork.GuardarAsync();
                return RedirectToAction("Index"); // Redirige al Index de Themes
            }
            return View(theme);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var theme = await _unitWork.theme.ObtenerAsync(id);
            if (theme == null)
            {
                return NotFound();
            }
            return View(theme);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Theme theme)
        {
            if (id != theme.ThemeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _unitWork.theme.Actualizar(theme);
                    await _unitWork.GuardarAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _unitWork.theme.ObtenerAsync(theme.ThemeId) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(theme);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var theme = await _unitWork.theme.ObtenerAsync(id);
            if (theme == null)
            {
                return NotFound();
            }
            return View(theme);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var theme = await _unitWork.theme.ObtenerAsync(id);
            _unitWork.theme.Eliminar(theme);
            await _unitWork.GuardarAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ApplyTheme(int id)
        {
            // Obtener el tema por su ID desde la base de datos
            var theme = await _unitWork.theme.ObtenerAsync(id);
            if (theme == null)
            {
                return NotFound();
            }
            // Guardar el ID del tema aplicado 
            HttpContext.Session.SetInt32("AppliedThemeId", id);

            return RedirectToAction(nameof(Index));
        }
    }
}
