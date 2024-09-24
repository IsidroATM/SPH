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
    public class ThemeConfiguration : IEntityTypeConfiguration<Theme>
    {
        public void Configure(EntityTypeBuilder<Theme> builder)
        {
            builder.ToTable("Themes"); // Nombre de la tabla en la base de datos
            builder.HasKey(t => t.ThemeId);
            builder.Property(t => t.Name)
                .HasMaxLength(20)
                .IsRequired();
            builder.Property(t => t.Color).IsRequired();
            builder.Property(t => t.FontType).IsRequired();
            builder.Property(t => t.FontSize).IsRequired();

            // Relación uno a muchos con User
            builder.HasOne(t => t.User)
                   .WithMany(u => u.Themes)
                   .HasForeignKey(t => t.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
