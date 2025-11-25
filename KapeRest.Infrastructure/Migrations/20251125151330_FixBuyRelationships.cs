using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBuyRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesItems_MenuItems_MenuItemId",
                table: "SalesItems");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesItems_MenuItems_MenuItemId",
                table: "SalesItems",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesItems_MenuItems_MenuItemId",
                table: "SalesItems");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesItems_MenuItems_MenuItemId",
                table: "SalesItems",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
