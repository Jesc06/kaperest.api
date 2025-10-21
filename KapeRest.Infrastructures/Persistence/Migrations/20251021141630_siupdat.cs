using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class siupdat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProduct_MenuItems_MenuItemId",
                table: "MenuItemProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProduct_Products_ProductOfSupplierId",
                table: "MenuItemProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemProduct",
                table: "MenuItemProduct");

            migrationBuilder.RenameTable(
                name: "MenuItemProduct",
                newName: "MenuItemProducts");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemProduct_ProductOfSupplierId",
                table: "MenuItemProducts",
                newName: "IX_MenuItemProducts_ProductOfSupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemProduct_MenuItemId",
                table: "MenuItemProducts",
                newName: "IX_MenuItemProducts_MenuItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemProducts",
                table: "MenuItemProducts",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProducts_MenuItems_MenuItemId",
                table: "MenuItemProducts",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_MenuItemProducts_MenuItems_MenuItemId",
                table: "MenuItemProducts");

            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemProducts",
                table: "MenuItemProducts");

            migrationBuilder.RenameTable(
                name: "MenuItemProducts",
                newName: "MenuItemProduct");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemProducts_ProductOfSupplierId",
                table: "MenuItemProduct",
                newName: "IX_MenuItemProduct_ProductOfSupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_MenuItemProducts_MenuItemId",
                table: "MenuItemProduct",
                newName: "IX_MenuItemProduct_MenuItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemProduct",
                table: "MenuItemProduct",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProduct_MenuItems_MenuItemId",
                table: "MenuItemProduct",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProduct_Products_ProductOfSupplierId",
                table: "MenuItemProduct",
                column: "ProductOfSupplierId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
