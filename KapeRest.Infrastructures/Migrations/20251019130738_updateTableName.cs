using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KapeRest.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class updateTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageName",
                table: "Products",
                newName: "ImageMimeType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageMimeType",
                table: "Products",
                newName: "ImageName");
        }
    }
}
