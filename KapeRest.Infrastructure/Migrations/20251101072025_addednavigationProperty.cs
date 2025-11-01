using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addednavigationProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SalesItems_MenuItemId",
                table: "SalesItems",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesItems_MenuItems_MenuItemId",
                table: "SalesItems",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesItems_MenuItems_MenuItemId",
                table: "SalesItems");

            migrationBuilder.DropIndex(
                name: "IX_SalesItems_MenuItemId",
                table: "SalesItems");
        }
    }
}
