﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using SPH.Models;
using SPH.Persistence;
using SPH.Repositories.Interfaces;
using SPH.Utilities;

namespace SPH.Controllers
{
    [Authorize]
    public class DiariesController : Controller
    {
        private readonly IUnitWork _unitWork;
        private readonly SPHDbContext _context;

        public DiariesController(IUnitWork unitWork, SPHDbContext context)
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
        public async Task<IActionResult> Index(string searchString)
        {
            try
            {
                int userId = GetUserId();
                ViewData["UserId"] = userId;

                var notes = await _unitWork.diary.GetUserNotesAsync(userId);

                if (!String.IsNullOrEmpty(searchString))
                {
                    notes = notes.Where(n => n.NombreNota.Contains(searchString, StringComparison.OrdinalIgnoreCase));
                }
                ViewData["CurrentFilter"] = searchString;
                return View(notes);
            }
            catch (Exception ex)
            {
                TempData[DS.Error] = "Se produjo un error al obtener las notas.";
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var diary = new Diary
            {
                FechaCreacion = DateTime.Now,
                UserId = GetUserId()
            };
            return View(diary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Diary diary)
        {
            if (!ModelState.IsValid)
            {
                return View(diary); // Retornamos la vista con el modelo en caso de error
            }

            diary.FechaCreacion = DateTime.Now;
            diary.UserId = GetUserId();

            await _unitWork.diary.AgregarAsync(diary);
            await _unitWork.GuardarAsync();
            TempData[DS.Successful] = "Nota creada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diary = await _unitWork.diary.ObtenerAsync(id.GetValueOrDefault());
            if (diary == null)
            {
                return NotFound();
            }

            return View(diary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Diary diary)
        {
            if (!ModelState.IsValid)
            {
                return View(diary); // Retornamos la vista con el modelo en caso de error
            }

            if (id != diary.NoteId)
            {
                return NotFound();
            }

            try
            {
                _unitWork.diary.Actualizar(diary);
                await _unitWork.GuardarAsync();
                TempData[DS.Successful] = "Nota actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View(diary); // Retornamos la vista con el modelo en caso de excepción
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diary = await _unitWork.diary.ObtenerAsync(id.GetValueOrDefault());
            if (diary == null)
            {
                return NotFound();
            }
            return View(diary);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var diary = await _unitWork.diary.ObtenerAsync(id.GetValueOrDefault());
            if (diary == null)
                return NotFound();
            return View(diary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!ModelState.IsValid)
            {
                var diary = await _unitWork.diary.ObtenerAsync(id);
                return View(diary); // Retornamos la vista con el modelo en caso de error
            }

            var diaryToDelete = await _unitWork.diary.ObtenerAsync(id);
            if (diaryToDelete == null)
            {
                return Json(new { success = false, message = "Nota no encontrada." });
            }

            _unitWork.diary.Eliminar(diaryToDelete);
            await _unitWork.GuardarAsync();

            return Json(new { success = true, message = "Nota eliminada correctamente." });
        }

        [HttpPost]
        public async Task<IActionResult> AutoSave(Diary diary)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Error al guardar automáticamente." });
            }

            if (diary.NoteId == 0) // Nueva nota
            {
                diary.FechaCreacion = DateTime.Now;
                await _unitWork.diary.AgregarAsync(diary);
            }
            else // Editando nota existente
            {
                var existingDiary = await _unitWork.diary.ObtenerAsync(diary.NoteId);
                if (existingDiary != null)
                {
                    existingDiary.NombreNota = diary.NombreNota;
                    existingDiary.Descripcion = diary.Descripcion;
                    // actualiza otros campos si es necesario
                    _unitWork.diary.Actualizar(existingDiary);
                }
            }

            await _unitWork.GuardarAsync();
            return Json(new { success = true, message = "Guardado automático realizado." });
        }
    }
}