using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class little : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductOfSupplierId1",
                table: "MenuItemProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemProducts_ProductOfSupplierId1",
                table: "MenuItemProducts",
                column: "ProductOfSupplierId1");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId1",
                table: "MenuItemProducts",
                column: "ProductOfSupplierId1",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId1",
                table: "MenuItemProducts");

            migrationBuilder.DropIndex(
                name: "IX_MenuItemProducts_ProductOfSupplierId1",
                table: "MenuItemProducts");

            migrationBuilder.DropColumn(
                name: "ProductOfSupplierId1",
                table: "MenuItemProducts");
        }
    }
}
