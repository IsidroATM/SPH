using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Models
{
    public class Theme
    {
        [Key]
        public int ThemeId { get; set; }

        [Required(ErrorMessage = "El nombre del tema es requerido")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El color del tema es requerido")]
        public string Color { get; set; }

        [Required(ErrorMessage = "El tipo de fuente es requerido")]
        public string FontType { get; set; }

        [Required(ErrorMessage = "El tamaño de fuente es requerido")]
        public int FontSize { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [NotMapped]
        public virtual User? User { get; set; }
    }
}
