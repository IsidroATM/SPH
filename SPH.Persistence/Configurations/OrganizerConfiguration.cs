using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SPH.Models;

namespace SPH.Persistence.Configurations
{
    public class OrganizerConfiguration : IEntityTypeConfiguration<Organizer>
    {
        public void Configure(EntityTypeBuilder<Organizer> builder)
        {
            builder.ToTable("Organizer");

            builder.HasKey(x => x.TaskId);

            builder.Property(x => x.Nombre)
                .HasMaxLength(200)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.Detalles)
                .HasMaxLength(1000)
                .IsUnicode()
                .IsRequired();

            builder.Property(x => x.FechaLimite)
                .IsRequired();

            builder.Property(x => x.FechaCreacion)
                .IsRequired();

            builder.Property(x => x.NivelPrioridad)
                .IsRequired();

            builder.Property(x => x.Estado)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithMany(u => u.Tareas)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
