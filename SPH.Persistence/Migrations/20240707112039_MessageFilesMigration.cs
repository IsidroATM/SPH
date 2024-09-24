using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPH.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MessageFilesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "MultimediaContent",
                table: "Messages",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MultimediaContentType",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MultimediaContent",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MultimediaContentType",
                table: "Messages");
        }
    }
}
