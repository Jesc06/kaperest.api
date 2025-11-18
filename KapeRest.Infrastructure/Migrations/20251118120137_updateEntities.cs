using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AffectedEntity",
                table: "AuditLog");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "AuditLog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AffectedEntity",
                table: "AuditLog",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "AuditLog",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
