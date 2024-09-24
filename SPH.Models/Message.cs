using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Models
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; }
      
        public byte[]? MultimediaContent { get; set; } // Propiedad para almacenar el contenido multimedia como datos binarios
    
        public string? MultimediaContentType { get; set; } // Tipo de contenido del archivo multimedia (opcional)

        [ForeignKey("Sender")]
        public int SenderId { get; set; }
        public virtual User? Sender { get; set; }

        [ForeignKey("Receiver")]
        public int ReceiverId { get; set; }
        public virtual User? Receiver { get; set; }

        [ForeignKey("Messenger")]
        public int MessengerId { get; set; }
        public virtual Messenger? Messenger { get; set; }
    }
}
