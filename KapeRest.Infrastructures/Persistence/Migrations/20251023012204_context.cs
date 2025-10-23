using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class context : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemProducts",
                table: "MenuItemProducts");

            migrationBuilder.DropIndex(
                name: "IX_MenuItemProducts_MenuItemId",
                table: "MenuItemProducts");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "MenuItemProducts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemProducts",
                table: "MenuItemProducts",
                columns: new[] { "MenuItemId", "ProductOfSupplierId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts",
                column: "ProductOfSupplierId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MenuItemProducts",
                table: "MenuItemProducts");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "MenuItemProducts",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MenuItemProducts",
                table: "MenuItemProducts",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItemProducts_MenuItemId",
                table: "MenuItemProducts",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItemProducts_Products_ProductOfSupplierId",
                table: "MenuItemProducts",
                column: "ProductOfSupplierId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
