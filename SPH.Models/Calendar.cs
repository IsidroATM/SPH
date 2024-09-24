using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Models
{
    public class Calendar
    {
        [Key]
        public int EventId { get; set; }

        [Required(ErrorMessage = "El nombre del evento es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre del evento debe tener entre 3 y 200 caracteres")]
        public string NombreEvento { get; set; }

        [Required(ErrorMessage = "El detalle del evento es requerido")]
        [StringLength(1000, ErrorMessage = "El detalle del evento no puede tener más de 1000 caracteres")]
        public string DetalleEvento { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaIniEvento { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFinEvento { get; set; }

        [Required(ErrorMessage = "La hora del evento es requerida")]
        public TimeSpan HoraEvento { get; set; }

        [Required(ErrorMessage = "El color del evento es requerido")]
        public string ColorEvento { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [NotMapped]
        public virtual User? User { get; set; }
    }
}
