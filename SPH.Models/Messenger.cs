using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Models
{
    public class Messenger
    {
        [Key]
        public int MessengerId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User? User { get; set; }

        [ForeignKey("UserContact")]
        public int UserContactId { get; set; }

        public virtual User? UserContact { get; set; }

        public string? NameUserContact { get; set; } // Nombre opcional del contacto
        
    }
}
