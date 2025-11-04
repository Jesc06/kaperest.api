using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateALlEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CashierId",
                table: "PendingUserAccount",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CashierId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CashierId",
                table: "AspNetUsers",
                column: "CashierId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CashierId",
                table: "AspNetUsers",
                column: "CashierId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_CashierId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_CashierId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CashierId",
                table: "PendingUserAccount");

            migrationBuilder.DropColumn(
                name: "CashierId",
                table: "AspNetUsers");
        }
    }
}
