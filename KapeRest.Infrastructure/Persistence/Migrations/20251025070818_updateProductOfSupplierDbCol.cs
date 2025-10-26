using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updateProductOfSupplierDbCol : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Stock",
                table: "Products",
                newName: "Stocks");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "CostPrice");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Products",
                newName: "Units");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Units",
                table: "Products",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Stocks",
                table: "Products",
                newName: "Stock");

            migrationBuilder.RenameColumn(
                name: "CostPrice",
                table: "Products",
                newName: "Price");
        }
    }
}
