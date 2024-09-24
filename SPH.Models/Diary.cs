using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Models
{
    public class Diary
    {
        [Key]
        public int NoteId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string NombreNota { get; set; }

        [Required(ErrorMessage = "La descripción es requerida")]
        [StringLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La fecha de creación es requerida")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [ForeignKey("User")]
        public int UserId { get; set; }

        [NotMapped]
        public virtual User? User { get; set; }
    }
}
