using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPH.Models;

namespace SPH.Persistence.Configurations 
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(x => x.Id);

            // Configurar los atributos
            builder.Property(x => x.Nombre)
                .HasMaxLength(200)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.NickName)
                .HasMaxLength(100)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.Edad)
                .IsRequired();

            builder.Property(x => x.Correo)
                .HasMaxLength(255)
                .IsUnicode()
                .IsRequired();  

            builder.HasIndex(u => u.Correo)
                .IsUnique();    // Único


            builder.Property(x => x.Contraseña)
                .IsRequired();
            //nuecos campos
            builder.Property(x => x.FechaNacimiento);

            builder.Property(x => x.ImagenPerfil)
                .HasMaxLength(200);

            builder.Property(x => x.Biografia)
                .HasMaxLength(500);

            builder.Property(x => x.NumeroContacto)
                .HasMaxLength(15);

            builder.Property(x => x.Ubicacion)
                .HasMaxLength(100);

            builder.Property(x => x.Rol)
                .IsRequired();

            builder.Property(x => x.Estado)
                .IsRequired();

            builder.Property(x => x.Organizacion)
                .HasMaxLength(100);
            //--------------------

            // Configuración de la relación uno a muchos con Note
            builder.HasMany(x => x.Notas)
                .WithOne(n => n.User)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación uno a muchos con Task (Tareas)
            builder.HasMany(u => u.Tareas)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación uno a muchos con eventos del calendario
            builder.HasMany(u => u.Eventos)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación uno a muchos con contactos de Messenger
            builder.HasMany(u => u.MessengerContacts)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación uno a muchos temas 
            builder.HasMany(u => u.Themes)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

