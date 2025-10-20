using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updateNameConvetion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Target",
                table: "AuditLog",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Module",
                table: "AuditLog",
                newName: "AffectedEntity");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Category",
                table: "AuditLog",
                newName: "Target");

            migrationBuilder.RenameColumn(
                name: "AffectedEntity",
                table: "AuditLog",
                newName: "Module");
        }
    }
}
