using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "VoucherUsages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCustomerSpecific",
                table: "Vouchers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContactNumber = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    LastPurchaseDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalPurchases = table.Column<int>(type: "int", nullable: false),
                    TotalSpent = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherUsages_CustomerId",
                table: "VoucherUsages",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_CustomerId",
                table: "Vouchers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_ContactNumber",
                table: "Customers",
                column: "ContactNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Vouchers_Customers_CustomerId",
                table: "Vouchers",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherUsages_Customers_CustomerId",
                table: "VoucherUsages",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vouchers_Customers_CustomerId",
                table: "Vouchers");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherUsages_Customers_CustomerId",
                table: "VoucherUsages");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_VoucherUsages_CustomerId",
                table: "VoucherUsages");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_CustomerId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "VoucherUsages");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "IsCustomerSpecific",
                table: "Vouchers");
        }
    }
}
