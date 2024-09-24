using System.ComponentModel.DataAnnotations;


namespace SPH.Models
{
    public class User
    {
        public int Id { get; set; }
        //--------------------------------------
        [Required(ErrorMessage = "El nombre es requerido")]
        [MaxLength(200, ErrorMessage = "El nombre no puede tener más de 200 caracteres")]
        [MinLength(3, ErrorMessage = "El nombre no debe tener menos de 3 caracteres")]
        public string Nombre { get; set; }
        //--------------------------------------
        [Required(ErrorMessage = "El nombre de usuario es requerido")]
        [MaxLength(100, ErrorMessage = "El nombre de usuario no puede tener más de 100 caracteres")]
        [MinLength(3, ErrorMessage = "El nombre de usuario no debe tener menos de 3 caracteres")]
        public string NickName { get; set; }
        //--------------------------------------
        [Required(ErrorMessage = "La edad es requerida")]
        [Range(1, 150, ErrorMessage = "La edad debe estar entre 1 y 150")]
        public int Edad { get; set; }
        //--------------------------------------
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        public string Correo { get; set; }
        //--------------------------------------
        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Contraseña { get; set; }
        //--------------------------------------- Nuevos campos
        public DateTime FechaNacimiento { get; set; }
        //--------------------------------------- Imágen de perfíl

        [MaxLength(200, ErrorMessage = "La URL de la imagen de perfil no puede tener más de 200 caracteres")]
        public string ImagenPerfil { get; set; }
        //---------------------------------------Descripción Bibliográfica
        [MaxLength(500, ErrorMessage = "La biografía no puede tener más de 500 caracteres")]
        public string Biografia { get; set; }
        //---------------------------------------Numero de contacto - Cel

        [MaxLength(15, ErrorMessage = "El número de contacto no puede tener más de 15 caracteres")]
        public string NumeroContacto { get; set; }
        //---------------------------------------Ubicación/Domicilio

        [MaxLength(100, ErrorMessage = "La ubicación no puede tener más de 100 caracteres")]
        public string Ubicacion { get; set; }
        //---------------------------------------
        // Campo para el rol del usuario
        [Required(ErrorMessage = "El rol es requerido")]
        public string Rol { get; set; }
        //---------------------------------------
        // Campo para el estado del usuario
        [Required(ErrorMessage = "El estado es requerido")]
        public bool Estado { get; set; }
        //---------------------------------------
        // Campo para la organización a la que pertenece el usuario
        [MaxLength(100, ErrorMessage = "El nombre de la organización no puede tener más de 100 caracteres")]
        public string Organizacion { get; set; }


        // Colección de notas  al usuario
        public virtual ICollection<Diary> Notas { get; set; } = new List<Diary>();

        // Colección de tareas  al usuario
        public virtual ICollection<Organizer> Tareas { get; set; } = new List<Organizer>();

        // Colección de eventos del calendario del usuario
        public virtual ICollection<Calendar> Eventos { get; set; } = new List<Calendar>();

        // Colección de contactos de mensajería del usuario
        public virtual ICollection<Messenger> MessengerContacts { get; set; } = new List<Messenger>();

        // Colección de mensajes enviados por el usuario
        public virtual ICollection<Message> SentMessages { get; set; } = new List<Message>();

        // Colección de mensajes recibidos por el usuario
        public virtual ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

        // Relación uno a muchos con Theme
        public virtual ICollection<Theme> Themes { get; set; } = new List<Theme>();

    }
}
