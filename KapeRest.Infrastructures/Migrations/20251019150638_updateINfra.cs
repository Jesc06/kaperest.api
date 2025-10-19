using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updateINfra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierTransactionHistory_Suppliers_SupplierId",
                table: "SupplierTransactionHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierTransactionHistory",
                table: "SupplierTransactionHistory");

            migrationBuilder.RenameTable(
                name: "SupplierTransactionHistory",
                newName: "SupplierTransactionHistories");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierTransactionHistory_SupplierId",
                table: "SupplierTransactionHistories",
                newName: "IX_SupplierTransactionHistories_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierTransactionHistories",
                table: "SupplierTransactionHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierTransactionHistories_Suppliers_SupplierId",
                table: "SupplierTransactionHistories",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierTransactionHistories_Suppliers_SupplierId",
                table: "SupplierTransactionHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierTransactionHistories",
                table: "SupplierTransactionHistories");

            migrationBuilder.RenameTable(
                name: "SupplierTransactionHistories",
                newName: "SupplierTransactionHistory");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierTransactionHistories_SupplierId",
                table: "SupplierTransactionHistory",
                newName: "IX_SupplierTransactionHistory_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierTransactionHistory",
                table: "SupplierTransactionHistory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierTransactionHistory_Suppliers_SupplierId",
                table: "SupplierTransactionHistory",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
