using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SPH.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Message1Migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessengerId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessengerId",
                table: "Messages",
                column: "MessengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messengers_MessengerId",
                table: "Messages",
                column: "MessengerId",
                principalTable: "Messengers",
                principalColumn: "MessengerId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messengers_MessengerId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessengerId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessengerId",
                table: "Messages");
        }
    }
}
