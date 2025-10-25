using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class editedDomainContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts",
                column: "ProductOfSupplierId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts",
                column: "ProductOfSupplierId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
