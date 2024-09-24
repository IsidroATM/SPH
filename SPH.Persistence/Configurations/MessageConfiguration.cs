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
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
           // Nombre de la tabla en la base de datos
            builder.ToTable("Messages");

            // Llave primaria
            builder.HasKey(m => m.MessageId);

            // Configuración de propiedades
            builder.Property(m => m.Content)
                .IsRequired();

            builder.Property(m => m.Timestamp)
                .IsRequired();

            // Propiedad MultimediaContent (datos binarios)
            builder.Property(m => m.MultimediaContent)
                .IsRequired(false); // Hacer opcional

            // Propiedad MultimediaContentType (tipo de contenido)
            builder.Property(m => m.MultimediaContentType)
                .IsRequired(false); // Hacer opcional

            // Relación con User (remitente)
            builder.HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada

            // Relación con User (destinatario)
            builder.HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict); // Restringir eliminación en cascada

            // Relación con Messenger
            builder.HasOne(m => m.Messenger)
                .WithMany()
                .HasForeignKey(m => m.MessengerId)
                .OnDelete(DeleteBehavior.Cascade); // Permitir eliminación en cascada
        }
    }
}
