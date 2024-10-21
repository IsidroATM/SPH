using Microsoft.AspNetCore.Mvc;
using SPH.Models;
using SPH.Repositories.Interfaces;
using System.Security.Claims;

namespace SPH.Controllers
{
    public class ThemeStylesController : Controller
    {
        private readonly IUnitWork _unitWork;

        public ThemeStylesController(IUnitWork unitWork)
        {
            _unitWork = unitWork;
        }

        public async Task<IActionResult> GetCurrentThemeStyles()
        {
            int? appliedThemeId = HttpContext.Session.GetInt32("AppliedThemeId");
            Theme currentTheme;
            if (appliedThemeId.HasValue)
            {
                currentTheme = await _unitWork.theme.ObtenerAsync(appliedThemeId.Value);
            }
            else
            {
                // Tema por defecto si no hay ninguno seleccionado
                currentTheme = new Theme
                {
                    Color = "#000000",
                    FontSize = 16,
                    FontType = "Arial, sans-serif"
                };
            }

            // Determinar el color de texto basado en el color de fondo
            var textColor = IsColorLight(currentTheme.Color) ? "#000000" : "#FFFFFF";

            var css = $@"
        body {{
            background-color: {currentTheme.Color};
            color: {textColor};
            font-size: {currentTheme.FontSize}px;
            font-family: {currentTheme.FontType};
        }}
    ";
            return Content(css, "text/css");
        }

        // Método auxiliar para determinar si un color es claro u oscuro
        private bool IsColorLight(string color)
        {
            // Convertir el color hex a RGB
            int r = Convert.ToInt32(color.Substring(1, 2), 16);
            int g = Convert.ToInt32(color.Substring(3, 2), 16);
            int b = Convert.ToInt32(color.Substring(5, 2), 16);

            // Calcular la luminosidad
            double luminance = (0.299 * r + 0.587 * g + 0.114 * b) / 255;

            return luminance > 0.5;
        }
    }
}