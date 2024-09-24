using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPH.Persistence.Configurations
{
    public class MessengerConfiguration : IEntityTypeConfiguration<Messenger>
    {
        public void Configure(EntityTypeBuilder<Messenger> builder)
        {
            builder.ToTable("Messengers");

            builder.HasKey(m => m.MessengerId);

            builder.Property(x => x.NameUserContact)
                .HasMaxLength(200)
                .IsUnicode();

            // Configuración de la relación con User (Usuario actual)
            builder.HasOne(m => m.User)
                .WithMany(u => u.MessengerContacts)
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de la relación con User (Usuario de contacto)
            builder.HasOne(m => m.UserContact)
                .WithMany()
                .HasForeignKey(m => m.UserContactId)
                .OnDelete(DeleteBehavior.Restrict); // Cambiado a Restrict
        }
    }
}
