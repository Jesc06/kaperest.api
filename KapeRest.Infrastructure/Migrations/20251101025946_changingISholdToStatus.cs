using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changingISholdToStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHold",
                table: "SalesTransaction");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "SalesTransaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "SalesTransaction");

            migrationBuilder.AddColumn<bool>(
                name: "IsHold",
                table: "SalesTransaction",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
