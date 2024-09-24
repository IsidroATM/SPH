using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPH.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CalendarMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEvento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DetalleEvento = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FechaIniEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoraEvento = table.Column<TimeSpan>(type: "time", nullable: false),
                    ColorEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_Events_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
