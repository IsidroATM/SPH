using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SPH.Models;

namespace SPH.Persistence.Configurations
{
    public class DiaryConfiguration : IEntityTypeConfiguration<Diary>
    {
        public void Configure(EntityTypeBuilder<Diary> builder)
        {
            // Nombre de la tabla
            builder.ToTable("Notes");

            // Clave primaria
            builder.HasKey(x => x.NoteId);

            // Configurar los atributos
            builder.Property(x => x.NombreNota)
                .HasMaxLength(200)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasMaxLength(1000)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.FechaCreacion)
                .IsRequired();

            // Configuración de la relación con el usuario
            builder.HasOne(x => x.User)
                .WithMany(u => u.Notas)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
