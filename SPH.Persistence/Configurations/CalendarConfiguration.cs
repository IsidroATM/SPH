using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPH.Models;


namespace SPH.Persistence.Configurations
{
    public class CalendarConfiguration : IEntityTypeConfiguration<Calendar>
    {
        public void Configure(EntityTypeBuilder<Calendar> builder)
        {
            // Nombre de la tabla
            builder.ToTable("Events");

            // Clave primaria
            builder.HasKey(x => x.EventId);

            // Configurar los atributos
            builder.Property(x => x.NombreEvento)
                .HasMaxLength(200)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.DetalleEvento)
                .HasMaxLength(1000)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.FechaIniEvento)
                .IsRequired();

            builder.Property(x => x.FechaFinEvento)
                .IsRequired();

            builder.Property(x => x.HoraEvento)
                .IsRequired();

            builder.Property(x => x.ColorEvento)
                .IsRequired();

            // Configuración de la relación con el usuario
            builder.HasOne(x => x.User)
                .WithMany(u => u.Eventos)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
