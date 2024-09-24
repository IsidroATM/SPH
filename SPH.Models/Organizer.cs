using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace SPH.Models
{
    public class Organizer
    {
        [Key]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los detalles son requeridos")]
        [StringLength(1000, ErrorMessage = "Los detalles no pueden tener más de 1000 caracteres")]
        public string Detalles { get; set; }

        [Required(ErrorMessage = "La fecha límite es requerida")]
        public DateTime FechaLimite { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "El nivel de prioridad es requerido")]
        public int NivelPrioridad { get; set; }


        [Required(ErrorMessage = "El estado de la tarea es requerido")]
        public string Estado { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [NotMapped]
        public virtual User? User { get; set; }

        [NotMapped]
        public string NivelPrioridadTexto
        {
            get
            {
                switch (NivelPrioridad)
                {
                    case 1:
                        return "Bajo";
                    case 2:
                        return "Medio";
                    case 3:
                        return "Alto";
                    default:
                        return "Desconocido";
                }
            }
        }
    }
}
 
